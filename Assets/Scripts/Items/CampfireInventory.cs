using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Items
{
    public class CampfireInventory : BlockInventory
    {

        public InventorySlot fuelSlot;
        public InventorySlot ingredientSlot;
        public InventorySlot outputSlot;

        List<Recipe> campfireRecipes = new List<Recipe>();

        //ui and polish purposes
        CampfireInventoryUI campfireInventoryUI;
        [SerializeField] Light2D campfireLight;
        [SerializeField] Animator spriteAnimator;

        int currentFlame = 0;
        bool isSmelting = false;

        const float SMELT_TIME = 2f;

        public override void OnSetupFinished()
        {
            //map the slots to part of the inventory
            fuelSlot = inventorySlots[0];
            ingredientSlot = inventorySlots[1];
            outputSlot = inventorySlots[2];

            campfireRecipes = RecipeManager.Instance.GetRecipiesOfType(RecipeType.Smelting);

            campfireInventoryUI = inventoryRoot.GetComponent<CampfireInventoryUI>();
        }

        private void Update()
        {
            if (!isSmelting)
            {
                SetFlame(currentFlame > 0);


                if (ingredientSlot.HasItem() &&
                    (!outputSlot.HasItem() || outputSlot.GetComponentInChildren<InventoryItem>().count <= ITEM_STACK_COUNT))
                {//if we have our ingredients
                    InventoryItem burnableItem = ingredientSlot.GetComponentInChildren<InventoryItem>();
                    //check if any recipes match the engredients
                    Recipe recipe = campfireRecipes.Find((Recipe r) =>
                    {
                        return (r.ingredients[0].item == burnableItem.item) //if the recipe has the burnable as it's ingredient
                        && (!outputSlot.HasItem()/* if we dont have an item in output or.. */
                        || r.product.item == outputSlot.GetComponentInChildren<InventoryItem>().item);//or the output slot is the product
                    });


                    if (recipe != null)
                    {
                        //if we don't have any flame, create flame
                        if (fuelSlot.HasItem() && currentFlame == 0 && fuelSlot.GetComponentInChildren<InventoryItem>().item.fuelStrength != 0)
                        {
                            RemoveItem(fuelSlot.GetComponentInChildren<InventoryItem>().item);//remove the item
                            currentFlame += fuelSlot.GetComponentInChildren<InventoryItem>().item.fuelStrength;
                            SetFlame(true);
                        }

                        if (currentFlame > 0)
                        {
                            //remove the ingredient and add the output
                            StartCoroutine(SmeltItem(recipe));
                        }
                    }
                }
            }
        }

        public void SetFlame(bool hasFlame)
        {
            campfireInventoryUI.SetFlame(hasFlame);
            campfireLight.enabled = hasFlame;
            spriteAnimator.SetInteger("FlameSize", currentFlame);
        }

        IEnumerator SmeltItem(Recipe recipe)
        {
            float timeLeft = SMELT_TIME;
            isSmelting = true;
            while(timeLeft >= 0)
            {
                timeLeft -= Time.deltaTime;
                campfireInventoryUI.SetProgress(timeLeft / SMELT_TIME);
                yield return null;
            }
            isSmelting = false;

            //check if recipe is still valid
            InventoryItem ingredient = ingredientSlot.GetComponentInChildren<InventoryItem>();
            if (ingredient != null && recipe.ingredients[0].item == ingredient.item)
            {
                currentFlame--;
                RemoveItem(ingredientSlot.GetComponentInChildren<InventoryItem>().item);//remove the ingredient
                AddItem(outputSlot, recipe.product.item, recipe.product.count);
            }
        }


    }
}