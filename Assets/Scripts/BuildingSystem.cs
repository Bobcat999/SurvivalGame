using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private TileBase hightlightTile;
    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap tempTilemap;

    [SerializeField] private GameObject lootPrafab;

    [SerializeField] private TileBase water;

    private Vector3Int playerPos;
    private Vector3Int highlightedTilePos;
    private bool highlighted;

    private void Update()
    {
        Item item = GameManager.Instance.playerInventory.GetSelectedItem(false);

        playerPos = mainTilemap.WorldToCell(transform.position);

        if (item != null)
        {
            HighlightTile(item);
        }
        else if (highlighted)
        {
            highlighted = false;
            tempTilemap.SetTile(highlightedTilePos, null);
        }

        //building and breaking
        if (!GameManager.Instance.IsInventoryOpen() && Mouse.current.leftButton.isPressed)
        {
            if (highlighted)
            {
                if (item.type == ItemType.Block)
                {
                    Build(highlightedTilePos, item);
                }
                else if (item.type == ItemType.Tool)
                {
                    DestroyBlock(highlightedTilePos);
                }
            }
        }
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
        Vector3Int mouseGridPos = GetMouseOnGridPos();

        tempTilemap.SetTile(highlightedTilePos, null);

        if (InRange(playerPos, mouseGridPos, currentItem.range))
        {

            if (groundTilemap.GetTile(mouseGridPos) != water && CheckCondition(mainTilemap.GetTile<BlockTile>(mouseGridPos), currentItem))
            {
                tempTilemap.SetTile(mouseGridPos, hightlightTile);
                highlightedTilePos = mouseGridPos;

                highlighted = true;
            }
            else
            {
                highlighted = false;
            }

        }
        else
        {
            highlighted = false;
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

    private bool CheckCondition(BlockTile tile, Item currentItem)
    {
        if (currentItem.type == ItemType.Block)
        {
            if (tile == null)
            {
                return true;
            }
        }
        else if (currentItem.type == ItemType.Tool)
        {
            if (tile)
            {
                if (tile.item.toolType == currentItem.toolType)
                {
                    return true;
                }
            }
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

    private void DestroyBlock(Vector3Int position)
    {
        //unhighlight
        tempTilemap.SetTile(position, null);
        highlighted = false;

        BlockTile tile = mainTilemap.GetTile<BlockTile>(position);
        mainTilemap.SetTile(position, null);

        Vector3 pos = mainTilemap.GetCellCenterWorld(position);
        GameObject loot = Instantiate(lootPrafab, pos, Quaternion.identity);
        loot.GetComponent<Loot>().Initialize(tile.item, tile.dropAmount);

    }
}
