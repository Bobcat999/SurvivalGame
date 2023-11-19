using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;


    public List<Recipe> recipes;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool CanCraft(Recipe recipe)
    {
        InventorySlot[] slots = GameManager.Instance.playerInventory.GetInventorySlots();

        bool haveAllIngredients = true;
        Debug.Log("Recipe: " + recipe.name);
        foreach (RecipeMaterial material in recipe.ingredients)
        {
            int count = material.count;
            Debug.Log("Material: " + material.item.name + " Need: " + count);
            foreach (InventorySlot slot in slots)
            {
                //get the item
                InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
                if (inventoryItem != null && material.item == inventoryItem.item)
                {
                    count -= inventoryItem.count;
                    Debug.Log(inventoryItem.item + " found, need " + count);
                    if (count <= 0)
                    {
                        continue;
                    }
                }
            }
            if (count > 0)
                haveAllIngredients = false;
            Debug.Log(haveAllIngredients ? "Have all ingredients so for" : "Don't have all ingredients");
        }

        //return if its craftable
        return haveAllIngredients;
    }

    //returns wheather or not it was crafted
    public bool CraftRecipe(Recipe recipe)
    {
        if (!CanCraft(recipe))
            return false;
        foreach (RecipeMaterial material in recipe.ingredients)
        {
            GameManager.Instance.playerInventory.RemoveItem(material.item, material.count);
        }
        GameManager.Instance.playerInventory.AddItem(recipe.product.item, recipe.product.count);
        return true;
    }

}
