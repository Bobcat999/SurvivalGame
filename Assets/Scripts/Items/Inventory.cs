using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

    public InventorySlot[] inventorySlots;
    public Transform inventoryRoot;
    public GameObject inventoryItemPrefab;
    public const int ITEM_STACK_COUNT = 64;
    public const int SLOTS_PER_ROW = 9;

    //returns true or false depending on if the item was succesfully added to inventory
    public bool AddItem(Item item)
    {
        //check if it can stack with any items
        if (item.stackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < ITEM_STACK_COUNT)
                {

                    itemInSlot.count++;
                    itemInSlot.RefreshCount();
                    return true;
                }
            }
        }

        //find empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        //failed to add to inventory
        return false;
    }


    InventoryItem SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
        return inventoryItem;
    }

    #region GettingAndLoadingData
    public InventorySlot[] GetInventorySlots()
    {
        return inventorySlots;
    }

    public InventoryData GetInventoryData()
    {
        InventoryData playerInventoryData = new InventoryData();
        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
            if (item == null)
            {
                playerInventoryData.slots.Add(new InventorySlotData("", 0));
                Debug.Log("Null Item");
                continue;
            }
            Debug.Log("Added item: " +  item.item.id + " count " + item.count);
            playerInventoryData.slots.Add(new InventorySlotData(item.item.id, item.count));
        }
        return playerInventoryData;
    }

    public void LoadInventoryData(InventoryData data)
    {
        SetInventorySlots(data.slots.ToArray());
    }

    //to assign the slots themselves
    public void SetInventorySlots(InventorySlot[] slots)
    {
        inventorySlots = slots;
    }

    //to assign values to existing slots
    public void SetInventorySlots(Item[] items, int[] counts)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //destroy current item if there is one
            if (inventorySlots[i].transform.GetChild(0))
            {
                Destroy(inventorySlots[i].transform.GetChild(0).gameObject);
            }
            //spawns the new item and sets the count
            SpawnNewItem(items[i], inventorySlots[i]).count = counts[i];
        }
    }

    public void SetInventorySlots(InventorySlotData[] slotDatas)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            //destroy current item if there is one
            if (inventorySlots[i].transform.childCount > 0)
            {
                Destroy(inventorySlots[i].transform.GetChild(0).gameObject);
            }
            //spawns the new item and sets the count
            if (WorldManager.Instance.GetItemFromItemId(slotDatas[i].itemId) != default)
            {
                SpawnNewItem(WorldManager.Instance.GetItemFromItemId(slotDatas[i].itemId), inventorySlots[i]).count = slotDatas[i].count;
            }
        }
    }
    #endregion


    public void ToggleInventory()
    {
        if(inventoryRoot.gameObject.activeSelf)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        inventoryRoot.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryRoot.gameObject.SetActive(false);
    }
}
