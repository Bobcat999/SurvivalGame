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
    public ActionType actionType;
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
    Weapon
}

public enum ActionType
{
    Mine,
    Chop,
}
