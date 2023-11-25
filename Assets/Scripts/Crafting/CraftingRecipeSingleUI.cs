using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeSingleUI : MonoBehaviour
{
    [SerializeField] GameObject recipeItemUIPrefab;
    [SerializeField] Transform ingredientsContainer;
    [SerializeField] Transform productContainer;

    [SerializeField] Button craftButton;


    [HideInInspector] public Recipe recipe;

    bool conditionCheckNeeded = false;

    private void Awake()
    {
        craftButton.onClick.AddListener(() => {//actually do the crafting
            if(!RecipeManager.Instance.CraftRecipe(recipe))
                conditionCheckNeeded = true;
            });
    }

    private void Start()
    {
        Inventory.OnInventoryChange += PlayerInventory_OnInventoryChange;
    }

    private void Update()
    {
        if (conditionCheckNeeded)
        {
            craftButton.interactable = RecipeManager.Instance.CanCraft(recipe);
            conditionCheckNeeded = false;
        }
    }

    private void PlayerInventory_OnInventoryChange()
    {
        if (recipe != null)
        {
            conditionCheckNeeded = true;
        }
    }

    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;

        foreach(RecipeMaterial material in recipe.ingredients)
        {
            GameObject materialTransform = Instantiate(recipeItemUIPrefab, ingredientsContainer);
            RecipeItemSingleUI recipeItem = materialTransform.GetComponent<RecipeItemSingleUI>();
            recipeItem.SetItemAndCount(material.item, material.count);
        }

        GameObject productTranform = Instantiate(recipeItemUIPrefab, productContainer);
        RecipeItemSingleUI productItem = productTranform.GetComponent<RecipeItemSingleUI>();
        productItem.SetItemAndCount(recipe.product.item, recipe.product.count);

        conditionCheckNeeded = true;
    }



}
