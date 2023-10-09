using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);

        foreach (Tilemap tilemap in tilemaps)
        {
            foreach (Tilemaps num in System.Enum.GetValues(typeof(Tilemaps)))
            {
                if (tilemap.name == num.ToString())
                {
                    if (!layers.ContainsKey((int)num)) layers.Add((int)num, tilemap);
                }
            }
        }
    }

    //world loading
    public const string worldFolder = "saves";
    public static BaseCommand loadCommand;
    public string currentWorldName;

    public static event Action OnLoadingStarted;
    public static event Action OnLoadingEnded;
    public static event Action OnCreateWorldStarted;
    public static event Action OnCreateWorldEnded;
    public static event Action OnSavingStarted;
    public static event Action OnSavingEnded;



    [SerializeField] List<Tilemap> tilemaps = new List<Tilemap>();
    public List<CustomTile> tiles = new List<CustomTile>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();

    public enum Tilemaps
    {
        Ground = 10,
        Surface = 20
    }

    public PlayerMovement player;

    public MapGeneration mapGeneration;

    public void Start()
    {
        loadCommand?.execute();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.E))
        {
            SaveCurrentWorld();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
        {
            LoadWorld(currentWorldName);
        }
    }

    public static string GetWorldsDirectory()
    {
        return Application.persistentDataPath + "/" + worldFolder;
    }

    public static void SetUpWorldsDirectory()
    {
        if (!Directory.Exists(GetWorldsDirectory()))
        {
            Directory.CreateDirectory(GetWorldsDirectory());
        }
    }

    public static void SetLoadCommand(BaseCommand command)
    {
        loadCommand = command;
    }

    public async void CreateNewWorld(string worldName, int seed)
    {
        OnCreateWorldStarted?.Invoke();

        currentWorldName = worldName;

        //generate the map
        await mapGeneration.GenerateMap(seed);
        //find a spawnpoint for the player
        player.transform.position = FindPlayerSpawnPos();

        OnCreateWorldEnded?.Invoke();

        //save the loaded world
        SaveWorldAs(worldName);

    }

    public Vector3 FindPlayerSpawnPos()
    {
        Tilemap groundMap = layers[(int)Tilemaps.Ground];
        TileBase water = tiles.Find(t => t.id == "water").tile;

        if (groundMap.GetTile(Vector3Int.zero) != water)
        {
            return Vector3.zero;
        }

        for (int size = 5; size < 50; size += 5)
        {
            for (int x = -size; x <= size; x++)
            {
                for (int y = -size; y <= size; y++)
                {
                    if (groundMap.GetTile(new Vector3Int(x, y, 0)) != water)
                    {
                        return new Vector3(x + .5f, y + .5f, 0);
                    }
                }
            }
        }

        return Vector3.zero;
    }

    public void SaveCurrentWorld()
    {
        SaveWorldAs(currentWorldName);
    }

    public void SaveWorldAs(string worldName)
    {
        Debug.Log("Started Saving");

        currentWorldName = worldName;

        WorldData worldData = new WorldData();

        foreach (int layer in layers.Keys)
        {
            worldData.layers.Add(new LayerData(layer));
        }

        //save the tilemaps
        foreach (LayerData layerData in worldData.layers)
        {
            if (!layers.TryGetValue(layerData.layerId, out Tilemap tilemap)) break;

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));

                    CustomTile tempTile = tiles.Find(t => t.tile == temp);

                    if (temp != null)
                    {
                        layerData.tiles.Add(tempTile.id);
                        layerData.posesX.Add(x);
                        layerData.posesY.Add(y);
                    }
                }
            }
        }

        //save the player position and stats
        worldData.playerPos = player.transform.position;

        string json = JsonUtility.ToJson(worldData, true);


        SetUpWorldsDirectory();

        File.WriteAllText(GetWorldsDirectory() + "/" + worldName + ".json", json);

        OnSavingEnded?.Invoke();
    }

    public async void LoadWorld(string worldName)
    {
        OnLoadingStarted?.Invoke();

        currentWorldName = worldName;

        string filePath = GetWorldsDirectory() + "/" + worldName + ".json";

        if (File.Exists(filePath))
        {
            string json = await ReadFileAsync(filePath);
            WorldData worldData = JsonUtility.FromJson<WorldData>(json);

            foreach (LayerData layerData in worldData.layers)
            {
                if (layers.TryGetValue(layerData.layerId, out Tilemap tilemap))
                {
                    // Clear tilemap
                    tilemap.ClearAllTiles();

                    for (int i = 0; i < layerData.tiles.Count; i++)
                    {
                        TileBase tile = tiles.Find(t => t.id == layerData.tiles[i]).tile;
                        if (tile)
                        {
                            tilemap.SetTile(new Vector3Int(layerData.posesX[i], layerData.posesY[i], 0), tile);
                        }
                    }
                }
            }

            // Set player position
            player.transform.position = worldData.playerPos;
        }
        else
        {
            Debug.LogError("World file not found: " + filePath);
        }

        OnLoadingEnded?.Invoke();
    }

    private async Task<string> ReadFileAsync(string filePath)
    {
        using (StreamReader reader = File.OpenText(filePath))
        {
            return await reader.ReadToEndAsync();
        }
    }

}

[System.Serializable]
public class WorldData
{
    public Vector3 playerPos = new Vector3();
    public List<LayerData> layers = new List<LayerData>();
}

[System.Serializable]
public class LayerData
{
    public int layerId;
    public List<string> tiles = new List<string>();
    public List<int> posesX = new List<int>();
    public List<int> posesY = new List<int>();

    public LayerData(int id)
    {
        layerId = id;
    }
}
