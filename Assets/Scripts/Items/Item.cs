using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    [Header("Loading Data")]
    public string id;

    [Header("Only Gameplay")]
    public TileBase tile;
    public ItemType type;
    public ToolType toolType;
    public Vector3Int range = new Vector3Int(5, 5, 0);

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;

}


public enum ItemType
{
    Block,
    Tool,
    Weapon,
    Item
}

public enum ToolType
{
    Pickaxe,
    Axe,
    Shovel
}
