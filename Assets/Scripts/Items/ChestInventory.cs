using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ChestInventory : Inventory
{
    //setup the chest
    private void Start()
    {
        GameManager.Instance.SetupChest(this);
    }

    //remove the chest
    public void OnDestroy()
    {
        if (GameManager.Instance.gameObject != null)
        {
            GameManager.Instance.RemoveChest(this);
        }
    }
}
