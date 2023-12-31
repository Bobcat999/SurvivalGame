using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomTiles/BlockTile")]
public class BlockTile : GameTile
{
    public Item item;

    public BreakType breakType = BreakType.None;
    public ToolMaterial minToolMaterial = ToolMaterial.None;
    public float breakTime = 1f;

    [SerializeField] int minDropAmount = 1;
    [SerializeField] int maxDropAmount = 1;

    public int dropAmount {  get { return Random.Range(minDropAmount, maxDropAmount); } private set {  maxDropAmount = value; } }

    public virtual void OnBlockBroken()
    {

    }

    public bool CanBreakBlock(Item handItem)
    {
        return handItem != null && (int)handItem.toolMaterial >= (int)minToolMaterial;
    }


}

public enum BreakType { Pickaxe, Axe, Sword, Unbreakable, None}

