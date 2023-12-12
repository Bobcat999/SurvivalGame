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
    public List<GameTile> tiles = new List<GameTile>();
    public List<Item> items = new List<Item>();
    public Dictionary<int, Tilemap> layers = new Dictionary<int, Tilemap>();

    public enum Tilemaps
    {
        Ground = 10,
        Main = 20
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
        return Path.Combine(Application.persistentDataPath, worldFolder);
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

    public void CreateNewWorld(string worldName, int seed)
    {
        StartCoroutine(CreateNewWorldAsync(worldName, seed));
    }

    public IEnumerator CreateNewWorldAsync(string worldName, int seed)
    {
        OnCreateWorldStarted?.Invoke();

        currentWorldName = worldName;

        //generate the map asyncrenously
        yield return StartCoroutine(mapGeneration.GenerateMapAsync(seed));


        //find a spawnpoint for the player
        player.transform.position = FindPlayerSpawnPos();

        //setup the players inventory
        //select a slot in the players inventory
        GameManager.Instance.playerInventory.SetSelectedSlot(0);
        //give player items to start with
        foreach (Item item in GameManager.Instance.startItems)
        {
            GameManager.Instance.playerInventory.AddItem(item, 1);
        }

        OnCreateWorldEnded?.Invoke();

        //save the loaded world

        SetUpWorldsDirectory();

        SaveWorldAs(worldName);

    }

    public Vector3 FindPlayerSpawnPos()
    {
        Tilemap groundMap = layers[(int)Tilemaps.Ground];
        TileBase water = tiles.Find(t => t.id == "water");

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
                    GameTile temp = tilemap.GetTile<GameTile>(new Vector3Int(x, y, 0));

                    if (temp != null)
                    {
                        layerData.tiles.Add(temp.id);
                        layerData.posesX.Add(x);
                        layerData.posesY.Add(y);
                    }
                }
            }
        }

        //save the player position and stats
        worldData.playerPos = player.transform.position;

        //save the player inventory and block inventories
        InventoryData playerInventoryData = GameManager.Instance.playerInventory.GetInventoryData();
        worldData.playerInventory = playerInventoryData;

        InventorySlotData playerHandData = PlayerHandManager.Instance.GetHandData();
        worldData.playerHand = playerHandData;

        //save the world time
        worldData.worldTime = TimeManager.instance.worldTime;

        //save the block inventories
        foreach(BlockInventory blockInventory in GameManager.Instance.blockInventories)
        {
            BlockInventoryData data = new BlockInventoryData();
            data.slots = blockInventory.GetInventoryData().slots;
            data.pos = layers[(int)Tilemaps.Main].WorldToCell(blockInventory.transform.position);
            worldData.blockInventories.Add(data);
        }


        //save the file to the computer
        string json = JsonUtility.ToJson(worldData, true);
        File.WriteAllText(Path.Combine(GetWorldsDirectory(), worldName + ".json"), json);

        OnSavingEnded?.Invoke();
    }

    public void LoadWorld(string worldName)
    {
        StartCoroutine(LoadWorldAsync(worldName));
    }

    public IEnumerator LoadWorldAsync(string worldName)
    {
        OnLoadingStarted?.Invoke();

        currentWorldName = worldName;

        string filePath = GetWorldsDirectory() + "/" + worldName + ".json";
        yield return null;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            WorldData worldData = JsonUtility.FromJson<WorldData>(json);

            foreach (LayerData layerData in worldData.layers)
            {
                if (layers.TryGetValue(layerData.layerId, out Tilemap tilemap))
                {
                    // Clear tilemap
                    tilemap.ClearAllTiles();

                    for (int i = 0; i < layerData.tiles.Count; i++)
                    {
                        TileBase tile = tiles.Find(t => t.id == layerData.tiles[i]);
                        if (tile)
                        {
                            tilemap.SetTile(new Vector3Int(layerData.posesX[i], layerData.posesY[i], 0), tile);
                        }
                    }
                }
                yield return null;
            }


            // Set player position
            player.transform.position = worldData.playerPos;

            //load the players inventory
            GameManager.Instance.playerInventory.LoadInventoryData(worldData.playerInventory);

            //load the players hand
            PlayerHandManager.Instance.LoadHandData(worldData.playerHand);

            //load the world time
            TimeManager.instance.worldTime = worldData.worldTime;

            //load the block inventories
            foreach (BlockInventoryData data in worldData.blockInventories)
            {
                BlockInventory blockInventory = layers[(int)Tilemaps.Main].GetInstantiatedObject(data.pos).GetComponent<BlockInventory>();
                GameManager.Instance.SetupBlockInventory(blockInventory);
                blockInventory.LoadInventoryData(data);
            }
        }
        else
        {
            Debug.LogError("World file not found: " + filePath);
        }

        OnLoadingEnded?.Invoke();
    }



    public Item GetItemFromItemId(string id)
    {
        return items.Find(i => i.id == id);
    }

}

[System.Serializable]
public class WorldData
{
    public Vector3 playerPos = new Vector3();
    public InventoryData playerInventory;
    public InventorySlotData playerHand;
    public float worldTime;
    public List<BlockInventoryData> blockInventories = new List<BlockInventoryData>();
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

[System.Serializable]
public class InventoryData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>();
}

[System.Serializable]
public class BlockInventoryData:InventoryData
{
    public Vector3Int pos;
}

[System.Serializable]
public class InventorySlotData
{
    public string itemId;
    public int count;

    public InventorySlotData(string itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }

    public static InventorySlotData NullSlotData()
    {
        return new InventorySlotData("", 0);
    }
}
