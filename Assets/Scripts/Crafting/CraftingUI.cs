using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] Transform recipesContainer;
    [SerializeField] Transform recipeTemplate;

    public void Start()
    {
        RefreshRecipes();
    }

    public void RefreshRecipes()
    {
        recipeTemplate.gameObject.SetActive(true);

        //remove all of the old recipes
        foreach(Transform child in recipesContainer)
        {
            if (child != recipeTemplate)
            {
                Destroy(child);
            }
        }

        //instantiate all of the recipes
        foreach(Recipe recipe in CraftingManager.Instance.recipes)
        {
            Transform recipeTransfrom = Instantiate(recipeTemplate, recipesContainer);
            CraftingRecipeSingleUI craftingRecipeSingleUI = recipeTransfrom.GetComponent<CraftingRecipeSingleUI>();
            craftingRecipeSingleUI.SetRecipe(recipe);
        }

        //hide the original
        recipeTemplate.gameObject.SetActive(false);
    }
}
