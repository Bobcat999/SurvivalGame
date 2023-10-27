using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Biome Preset", menuName = "New Biome Preset")]
public class BiomePreset : ScriptableObject
{
    public TileBase groundTile;
    public float minHeight;
    public float minMoisture;
    public float minHeat;
    public float minVegetation;

    public TileBase GetTile()
    {
        return groundTile;
    }

    public bool MatchCondition(float height, float moisture, float heat, float vegetation)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat && vegetation >= minVegetation;
    }
}