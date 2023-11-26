using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class CampfireInventory : BlockInventory
    {

        public InventorySlot fuelSlot;
        public InventorySlot burnableSlot;
        public InventorySlot outputSlot;

        List<Recipe> campfireRecipes = new List<Recipe>();

        float currentFlame = 0f;

        const float SMELT_TIME = 2f;

        public override void OnSetupFinished()
        {
            //map the slots to part of the inventory
            fuelSlot = inventorySlots[0];
            burnableSlot = inventorySlots[1];
            outputSlot = inventorySlots[2];

            campfireRecipes = RecipeManager.Instance.GetRecipiesOfType(RecipeType.Campfire);
        }

        private void Update()
        {
            if((fuelSlot.HasItem() && (burnableSlot.HasItem() || currentFlame >= 1)))
            {//if we have our ingredients
                InventoryItem burnableItem = burnableSlot.GetComponentInChildren<InventoryItem>();
                //check if any recipes match the engredients
                Recipe recipe = campfireRecipes.Find((Recipe r) =>
                {
                    return (r.ingredients[0].item == burnableItem.item) 
                    && (!outputSlot.HasItem() || r.product.item == outputSlot.GetComponentInChildren<InventoryItem>().item);
                });

                if(recipe != null)
                {

                }
            }
        }


    }
}