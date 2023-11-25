using Assets.Scripts.Commands;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private TileBase hightlightTile;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap tempTilemap;

    [SerializeField] private GameObject lootPrafab;

    [SerializeField] private TileBase water;//nothing can be highlighted above water

    private Vector3Int playerPos;
    private Vector3Int highlightedTilePos;
    private bool highlighted;//to check if the item being heald's action can be preformed
    [SerializeField] Item fistItem;

    public BreakCommand currentBreakCommand;

    private PlayerControls controlls;

    public static BuildingSystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



    private void Start()
    {
        //handle input
        controlls = new PlayerControls();
        controlls.Player.Enable();
        controlls.Player.Interact.performed += Interact_performed;

    }

    private void OnDestroy()
    {
        controlls.Player.Interact.performed -= Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        Item item = GetSelectedItem();
        if (CheckPlaceCondition(mainTilemap.GetTile<BlockTile>(highlightedTilePos), item))//if you can preform an action
        {
            Build(highlightedTilePos, item);
        }
    }


    private void StartNewBreak()
    {
        BlockTile tile = mainTilemap.GetTile<BlockTile>(highlightedTilePos);
        if (CheckBreakCondition(tile))
        {
            currentBreakCommand = new BreakCommand(GetSelectedItem(), tile, highlightedTilePos);
        }

    }

    private Item GetSelectedItem()
    {
        Item item = GameManager.Instance.playerInventory.GetSelectedItem(false);//get the current item without using it
        if (item == null)
            item = fistItem;
        return item;
    }

    private void EndCurrentBreak()
    {
        currentBreakCommand = null;
    }

    private void Update()
    {
        if (!IsMenuOpen())
        {


            playerPos = mainTilemap.WorldToCell(transform.position);//save the player position

            //LOGIC TO CHECK IF WE CAN PREFORM AN ACTION

            HighlightTile(GetSelectedItem());
        }

        //start or end any Break commands
        if (currentBreakCommand != null &&
            (!highlighted
            || !controlls.Player.Mine.IsPressed()
            || currentBreakCommand.tilePos != highlightedTilePos
            || GetSelectedItem() != currentBreakCommand.tool))//END THE COMMAND
        {
            EndCurrentBreak();
        }

        if (currentBreakCommand == null && controlls.Player.Mine.IsPressed())//START A NEW COMMAND
        {
            StartNewBreak();
        }

        //CHECK IF DONE BREAKING
        if (currentBreakCommand != null && currentBreakCommand.IsFinished())
        {
            DestroyBlock(highlightedTilePos, currentBreakCommand.dropContents);
            EndCurrentBreak();
        }
    }

    private bool IsMenuOpen()
    {
        return GameManager.Instance.IsInventoryOpen();
    }

    public bool IsBreaking()
    {
        return currentBreakCommand != null;
    }

    private Vector3Int GetMouseOnGridPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = mainTilemap.WorldToCell(mousePos);

        mouseCellPos.z = 0;

        return mouseCellPos;

    }

    private void HighlightTile(Item currentItem)
    {
        Vector3Int mouseGridPos = GetMouseOnGridPos();//gets the mouse position

        tempTilemap.SetTile(highlightedTilePos, null);//sets the old highlight to null

        //make sure its in range, the tile underneath isnt water and check if the conditions are right for it to get highlighted
        if (InRange(playerPos, mouseGridPos, currentItem.range) && groundTilemap.GetTile(mouseGridPos) != water && CheckHighlightCondition(mainTilemap.GetTile<BlockTile>(mouseGridPos), currentItem))
        {
            //highlight the tile
            tempTilemap.SetTile(mouseGridPos, hightlightTile);
            if (highlightedTilePos != mouseGridPos)
            {
                highlightedTilePos = mouseGridPos;
            }

            highlighted = true;
        }
        else
        {
            highlighted = false;//unhighlight
        }

    }

    private bool InRange(Vector3Int positionA, Vector3Int positionB, Vector3Int range)
    {
        Vector3Int distance = positionA - positionB;

        if (Mathf.Abs(distance.x) >= range.x ||
           Mathf.Abs(distance.y) >= range.y)
        {
            return false;
        }

        return true;
    }

    private bool CheckHighlightCondition(BlockTile tile, Item currentItem)
    {

        if (currentItem.type != ItemType.Block)
        {
            if (tile == null)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckPlaceCondition(BlockTile tile, Item currentItem)
    {
        if (!highlighted)
            return false;

        if (currentItem.type == ItemType.Block && tile == null)
        {
            return true;
        }

        return false;
    }

    private bool CheckBreakCondition(BlockTile tile)
    {
        if (!highlighted)
            return false;

        if (tile != null)
        {
            return true;
        }

        return false;
    }

    private void Build(Vector3Int position, Item itemToBuild)
    {
        //unhighlight
        tempTilemap.SetTile(position, null);
        highlighted = false;

        //remove item from inventory
        GameManager.Instance.playerInventory.GetSelectedItem(true);

        mainTilemap.SetTile(position, itemToBuild.tile);
    }

    private void DestroyBlock(Vector3Int position, bool dropContents)
    {
        //unhighlight
        tempTilemap.SetTile(position, null);
        highlighted = false;


        BlockTile tile = mainTilemap.GetTile<BlockTile>(position);
        //run anything we need on the block
        tile.OnBlockBroken();

        //if there is a block inventory, drop the loot
        GameObject instantiatedObj = mainTilemap.GetInstantiatedObject(position);
        if (instantiatedObj != null && instantiatedObj.GetComponent<BlockInventory>())
        {
            instantiatedObj.GetComponent<BlockInventory>().OnInventoryDestroyed();
        }

        //set the tile to null
        mainTilemap.SetTile(position, null);

        //drop the tile loot
        Vector3 pos = mainTilemap.GetCellCenterWorld(position);
        GameObject loot = Instantiate(lootPrafab, pos, Quaternion.identity);
        loot.GetComponent<Loot>().Initialize(tile.item, tile.dropAmount);


    }
}
