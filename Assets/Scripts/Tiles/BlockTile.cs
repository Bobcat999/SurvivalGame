using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomTiles/BlockTile")]
public class BlockTile : GameTile
{
    public Item item;

    public BreakType breakType;

    [SerializeField] int minDropAmount = 1;
    [SerializeField] int maxDropAmount = 1;

    public int dropAmount {  get { return Random.Range(minDropAmount, maxDropAmount); } private set {  maxDropAmount = value; } }


}

public enum BreakType { Pickaxe, Axe, Sword, Unbreakable}

