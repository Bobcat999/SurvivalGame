using System.Collections;
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
    public Vector2Int offset;

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


    [Header("Recourses")]
    public RecourseGen[] recourceList;


    private void Update()
    {
    }

    public IEnumerator GenerateMapAsync(int seed)
    {

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

        // Retrieve the generated maps
        heightMap = ProceduralGeneration.Generate(width, height, scale, heightWaves, offset);
        yield return null;
        moistureMap = ProceduralGeneration.Generate(width, height, scale, moistureWaves, offset);
        yield return null;
        heatMap = ProceduralGeneration.Generate(width, height, scale, heatWaves, offset);
        yield return null;
        vegetationMap = ProceduralGeneration.Generate(width, height, scale, vegetationWaves, offset);
        yield return null;

        //generate water boarder
        for (int x = -1; x < width + 1; ++x)
        {
            for (int y = -1; y < height + 1; ++y)
            {
                if ((x >= width || x < 0) || (y >= height || y < 0))
                {
                    groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), groundBiomes[0].GetTile());
                }
            }
        }
        yield return null;

        //generate ground
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], vegetationMap[x, y], groundBiomes).GetTile();
                groundMap.SetTile(new Vector3Int(x + (int)offset.x, y + (int)offset.y, 0), tile);
            }
            yield return null;
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
                    TileBase tile = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y], vegetationMap[x, y], treeBiomes).GetTile();
                    mainMap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), tile);
                }
            }
            yield return null;
        }

        //generate recouses
        foreach (RecourseGen recourse in recourceList)
        {
            for (int i = 0; i < recourse.numGens; i++)
            {
                int x;
                int y;
                do
                {
                    x = Random.Range(0, width);
                    y = Random.Range(0, height);
                } while (groundMap.GetTile(new Vector3Int(x + offset.x, y + offset.y, 0)) == groundBiomes[0].GetTile());

                int numRecourses = Random.Range(recourse.minPerGen, recourse.maxPerGen);

                for (int j = 0; j < numRecourses; j++)
                {
                    int recourseX;
                    int recourseY;

                    do {
                        recourseX = x + Random.Range(-recourse.spread, recourse.spread);
                        recourseY = y + Random.Range(-recourse.spread, recourse.spread);

                    } while (groundMap.GetTile(new Vector3Int(recourseX + offset.x, recourseY + offset.y, 0)) == null 
                    || groundMap.GetTile(new Vector3Int(recourseX + offset.x, recourseY + offset.y, 0)) == groundBiomes[0].GetTile());

                    mainMap.SetTile(new Vector3Int(recourseX + offset.x, recourseY + offset.y, 0), recourse.tile);


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
                float diffVal = biome.GetDiffValue(height, moisture, heat, vegetation);
                if (diffVal < curVal)
                {
                    biomeToReturn = biome.biome;
                    curVal = diffVal;
                }
                else if (diffVal == curVal)
                {
                    biomeToReturn = (Random.Range(0f, 1f) > .5f) ? biome.biome : biomeToReturn;
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


[System.Serializable]
public class RecourseGen
{
    public BlockTile tile;
    public int numGens;
    public int maxPerGen = 1;
    public int minPerGen = 1;
    public int spread;
}