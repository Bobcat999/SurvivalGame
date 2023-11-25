using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "CustomTiles/Biome Changing Tile")]
public class BiomeChangingTile : Tile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //get these values from the base class
        base.GetTileData(position, tilemap, ref tileData);

        tileData.sprite = GetSprite(position);
    }


    public Dictionary<TileBase, Sprite> biomeSprites;
    public Tilemap groundTileMap;

    public Sprite GetSprite(Vector3Int pos)
    {
        return biomeSprites[groundTileMap.GetTile(pos)];
    }
}