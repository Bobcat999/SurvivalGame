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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMapAsync(Random.Range(0,1000));
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

        // Wait for all calculations to complete
        await Task.WhenAll(heightMapTask, moistureMapTask, heatMapTask);

        // Retrieve the generated maps
        heightMap = heightMapTask.Result;
        moistureMap = moistureMapTask.Result;
        heatMap = heatMapTask.Result;

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
                TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], groundBiomes).GetTile();
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
                if (groundMap.GetTile(new Vector3Int(x, y, 0)) != groundBiomes[0].GetTile())
                {
                    TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], treeBiomes).GetTile();
                    mainMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), tile);
                }
            }
        }
    }

    BiomePreset GetBiome(float height, float moisture, float heat, BiomePreset[] biomes)
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