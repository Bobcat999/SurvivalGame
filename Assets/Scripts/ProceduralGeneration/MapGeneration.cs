﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapGeneration : MonoBehaviour
{

    public BiomePreset[] groundBiomes;
    public BiomePreset[] treeBiomes;
    public Tilemap groundMap;
    public Tilemap mainMap;

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
    [Header("Vegetation Map")]
    public Wave[] vegetationWaves;
    public float vegetationWaveOffset;
    private float[,] vegetationMap;


    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            await GenerateMapAsync(Random.Range(0,1000));
        }
    }

    public async Task GenerateMapAsync(int seed)
    {
        await Task.Yield(); // Yield briefly to ensure this method is asynchronous.

        // GETS ALL OF THE SEEDS
        foreach (Wave wave in heightWaves)
        {
            wave.seed = seed * heightWaveOffest;
        }
        foreach (Wave wave in moistureWaves)
        {
            wave.seed = seed * moistureWaveOffest;
        }
        foreach (Wave wave in heatWaves)
        {
            wave.seed = seed * heatWaveOffest;
        }
        foreach (Wave wave in vegetationWaves)
        {
            wave.seed = seed * vegetationWaveOffset;
        }

        //CALCULATES ALL OF THE MAPS
        // height map (Calculate asynchronously)
        Task<float[,]> heightMapTask = Task.Run(() =>
        {
            return ProceduralGeneration.Generate(width, height, scale, heightWaves, offset);
        });

        // moisture map (Calculate asynchronously)
        Task<float[,]> moistureMapTask = Task.Run(() =>
        {
            return ProceduralGeneration.Generate(width, height, scale, moistureWaves, offset);
        });

        // heat map (Calculate asynchronously)
        Task<float[,]> heatMapTask = Task.Run(() =>
        {
            return ProceduralGeneration.Generate(width, height, scale, heatWaves, offset);
        });

        // vegetation map (Calculate asynchronously)
        Task<float[,]> vegetationMapTask = Task.Run(() =>
        {
            return ProceduralGeneration.Generate(width, height, scale, vegetationWaves, offset);
        });

        // Wait for all calculations to complete
        await Task.WhenAll(heightMapTask, moistureMapTask, heatMapTask, vegetationMapTask);

        // Retrieve the generated maps
        heightMap = heightMapTask.Result;
        moistureMap = moistureMapTask.Result;
        heatMap = heatMapTask.Result;
        vegetationMap = vegetationMapTask.Result;

        //generate water boarder
        for (int x = -1; x < width + 1; ++x)
        {
            for (int y = -1; y < height + 1; ++y)
            {
                groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), groundBiomes[0].GetTile());
            }
        }

        //generate ground
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], vegetationMap[x,y], groundBiomes).GetTile();
                groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), tile);
            }
        }
        //generate surface things

        mainMap.ClearAllTiles();

        //generate trees
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                TileBase groundTile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], vegetationMap[x, y], groundBiomes).GetTile();
                if (groundTile != null && groundTile != groundBiomes[0].GetTile())
                {
                    TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], vegetationMap[x,y], treeBiomes).GetTile();
                    mainMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), tile);
                }
            }
        }
    }

    BiomePreset GetBiome(float height, float moisture, float heat, float vegetation, BiomePreset[] biomes)
    {
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();
        foreach (BiomePreset biome in biomes)
        {
            if (biome.MatchCondition(height, moisture, heat, vegetation))
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
                curVal = biome.GetDiffValue(height, moisture, heat, vegetation);
            }
            else
            {
                if (biome.GetDiffValue(height, moisture, heat, vegetation) < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = biome.GetDiffValue(height, moisture, heat, vegetation);
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

    public float GetDiffValue(float height, float moisture, float heat, float vegetation)
    {
        return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat) + (vegetation - biome.minVegetation);
    }
}