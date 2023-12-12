using System.Collections;
using UnityEngine;

public class BlockInventory : Inventory
{

    public GameObject inventoryUIPrefab;

    //setup the inventory
    private void Start()
    {
        GameManager.Instance.SetupBlockInventory(this);
        OnSetupFinished();
    }

    //remove the chest
    public void OnInventoryDestroyed()
    {
        GameManager.Instance.RemoveBlockInventory(this);

        //drop block contents on ground
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
            if (item != null)
            {
                GameObject loot = Instantiate(GameManager.Instance.lootPrefab, transform.position, Quaternion.identity);
                loot.GetComponent<Loot>().Initialize(item.item, item.count);
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemoveBlockInventory(this);
    }

    public virtual void OnSetupFinished()
    {

    }
}