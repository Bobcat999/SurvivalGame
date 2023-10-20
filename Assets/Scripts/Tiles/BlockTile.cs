using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomTiles/BlockTile")]
public class BlockTile : AdvancedRuleTile
{
    public Item item;

    [SerializeField] int minDropAmount = 1;
    [SerializeField] int maxDropAmount = 1;

    public int dropAmount {  get { return Random.Range(minDropAmount, maxDropAmount); } private set {  maxDropAmount = value; } }


}

