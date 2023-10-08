using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;
    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else Destroy(this);
    }

    //world loading
    public const string worldFolder = "saves";
    public static BaseCommand loadCommand;


    public List<Tilemap> tilemaps;
    public List<CustomTile> tiles = new List<CustomTile>();

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
            SaveWorld("testWorld");
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
        {
            LoadWorld("testWorld");
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

    public void CreateNewWorld(string worldName, int seed)
    {
        mapGeneration.GenerateMap(seed);
        SaveWorld(worldName);
    }

    public void SaveWorld(string worldName)
    {

        Debug.Log("Started Saving");


        WorldData worldData = new WorldData();

        //save the tilemaps
        for(int i = 0; i < tilemaps.Count; i++) {

            Tilemap tilemap = tilemaps[i];

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tilemap.GetTile(new Vector3Int(x, y, 0));

                    CustomTile tempTile = tiles.Find(t => t.tile==temp);

                    if (temp != null)
                    {
                        worldData.tiles.Add(tempTile.id);
                        worldData.poses.Add(new Vector3Int(x, y, i));
                    }
                }
            }
        }

        //save the player position and stats
        worldData.playerPos = player.transform.position; 

        string json = JsonUtility.ToJson(worldData, true);
        SetUpWorldsDirectory();

        File.WriteAllText(GetWorldsDirectory() + "/" + worldName + ".json", json);

        Debug.Log("Level Was Saved");
    }

    public void LoadWorld(string worldName)
    {
        string json = File.ReadAllText(GetWorldsDirectory() + "/" + worldName + ".json");
        WorldData data = JsonUtility.FromJson<WorldData>(json);

        //load tilemap
        tilemaps.ForEach(tilemap => { tilemap.ClearAllTiles(); });

        for (int i = 0; i < data.tiles.Count; i++)
        {
            tilemaps[data.poses[i].z].SetTile(data.poses[i], tiles.Find(t => t.id == data.tiles[i]).tile);
        }

        //set player position
        player.transform.position = data.playerPos;
        
    }
}

public class WorldData
{
    public List<string> tiles = new List<string>();
    public List<Vector3Int> poses = new List<Vector3Int>();
    public Vector3 playerPos = new Vector3();
}
