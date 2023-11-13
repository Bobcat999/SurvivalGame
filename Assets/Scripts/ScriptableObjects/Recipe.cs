using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObject/Recipe")]
public class Recipe : ScriptableObject
{
    public RecipeMaterial[] ingredients;
    public RecipeMaterial product;


}

[System.Serializable]
public class RecipeMaterial
{
    public Item item;
    public int count;
}
