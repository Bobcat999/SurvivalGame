using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapGeneration : MonoBehaviour
{

    public BiomePreset[] biomes;
    public Tilemap groundMap;
    public Tilemap surfaceMap;

    [Header("Dimensions")]
    public int width = 50;
    public int height = 50;
    public float scale = 1.0f;
    public Vector2 offset;

    [Header("Seed")]
    public float seed;

    [Header("Height Map")]
    public Wave[] heightWaves;
    public float heightWaveOffest;
    public float[,] heightMap;
    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    public float moistureWaveOffest;
    private float[,] moistureMap;
    [Header("Heat Map")]
    public Wave[] heatWaves;
    public float heatWaveOffest;
    private float[,] heatMap;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap(Random.Range(0,1000));
        }
    }

    public void GenerateMap(int seed)
    {
        //set the seeds to random numbers
        foreach (Wave wave in heightWaves)
        {
            wave.seed = seed * heightWaveOffest;
        }
        foreach(Wave wave in moistureWaves)
        {
            wave.seed = seed * moistureWaveOffest;
        }
        foreach(Wave wave in heatWaves)
        {
            wave.seed = seed * heatWaveOffest;
        }

        // height map
        heightMap = ProceduralGeneration.Generate(width, height, scale, heightWaves, offset);
        // moisture map
        moistureMap = ProceduralGeneration.Generate(width, height, scale, moistureWaves, offset);
        // heat map
        heatMap = ProceduralGeneration.Generate(width, height, scale, heatWaves, offset);

        //generate water boarder
        for (int x = -1; x < width+1; ++x)
        {
            for (int y = -1; y < height+1; ++y)
            {
                groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), biomes[0].GetTile());
            }
        }

        //generate map
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).GetTile();
                groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y,0), tile);
            }
        }

        //generate surface things
        surfaceMap.ClearAllTiles();
    }

    BiomePreset GetBiome(float height, float moisture, float heat)
    {
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();
        foreach (BiomePreset biome in biomes)
        {
            if (biome.MatchCondition(height, moisture, heat))
            {
                biomeTemp.Add(new BiomeTempData(biome));
            }
        }

        BiomePreset biomeToReturn = null;
        float curVal = 0.0f;
        foreach (BiomeTempData biome in biomeTemp)
        {
            if (biomeToReturn == null)
            {
                biomeToReturn = biome.biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
            else
            {
                if (biome.GetDiffValue(height, moisture, heat) < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat);
                }
            }
        }
        if (biomeToReturn == null)
            biomeToReturn = biomes[0];
        return biomeToReturn;
    }
}

public class BiomeTempData
{
    public BiomePreset biome;
    public BiomeTempData(BiomePreset preset)
    {
        biome = preset;
    }

    public float GetDiffValue(float height, float moisture, float heat)
    {
        return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
    }
}