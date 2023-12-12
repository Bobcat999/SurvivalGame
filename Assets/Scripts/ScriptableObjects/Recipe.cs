using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObject/Recipe")]
public class Recipe : ScriptableObject
{
    public RecipeMaterial[] ingredients;
    public RecipeMaterial product;

    public RecipeType type = RecipeType.Crafting;
}

[System.Serializable]
public class RecipeMaterial
{
    public Item item;
    public int count;
}

public enum RecipeType { Crafting, Smelting}
