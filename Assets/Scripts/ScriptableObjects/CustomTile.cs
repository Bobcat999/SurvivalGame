using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomTile", menuName = "ScriptableObject/CustomTile")]
public class CustomTile : ScriptableObject
{
    public TileBase tile;
    public string id;
    public WorldManager.Tilemaps tilemap;
}
