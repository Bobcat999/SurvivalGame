using System;
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


    //events
    public static event Action OnInventoryChange;
    public static void InventoryChanged()
    {
        OnInventoryChange?.Invoke();
    }


    //returns the amount of items that couldnt be put into the inventory
    public int AddItem(Item item, int count = 1)
    {
        //tell everything that we changed the inventory
        Inventory.OnInventoryChange?.Invoke();

        //if the count is already 0, just return
        if (count == 0)
            return 0;


        //check if it can stack with any items
        if (item.stackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)//list through the inventory slots
            {
                InventorySlot slot = inventorySlots[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

                if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < ITEM_STACK_COUNT)
                {
                    //get the amount of space the slot has left
                    int space = ITEM_STACK_COUNT - itemInSlot.count;

                    if (count <= space)//add the items to the stack if the stack has the space
                    {
                        itemInSlot.count += count;
                        itemInSlot.RefreshCount();
                        return 0;
                    }
                    else//make it a full stack and make a new stack of the remaining items
                    {
                        itemInSlot.count += space;
                        itemInSlot.RefreshCount();
                        return AddItem(item, count-space);
                    }
                }
            }
        }

        //find empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)//if there is not iem in the slot
            {
                if (item.stackable && count <= ITEM_STACK_COUNT)
                {
                    SpawnNewItem(item, slot).count = count;
                    return 0;
                }
                else if (item.stackable && count > ITEM_STACK_COUNT)
                {
                    SpawnNewItem(item, slot).count = ITEM_STACK_COUNT;
                    return AddItem(item, count - ITEM_STACK_COUNT);
                }
                else
                {
                    SpawnNewItem(item, slot);
                    return AddItem(item, count - 1);
                }
            }
        }
        //failed to add to inventory
        return count;
    }

    //returns the amount of items that fialed to be put into the inventory
    public int RemoveItem(Item item, int count = 1)
    {
        //tell everything that we changed the inventory
        Inventory.OnInventoryChange?.Invoke();

        //if the count is already 0, just return
        if (count == 0)
            return 0;

        //find any existing stacks
        foreach(InventorySlot slot in inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot != null && itemInSlot.item == item)
            {
                if(itemInSlot.count > count)//if its less than the stack just remove it from there and return
                {
                    itemInSlot.count -= count;
                    itemInSlot.RefreshCount();
                    return 0;
                }
                else//otherwhise delete that stack and remove the remaining
                {
                    count -= itemInSlot.count;
                    Destroy(itemInSlot.gameObject);
                    return RemoveItem(item, count);
                }
            }
        }

        return count;
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
                playerInventoryData.slots.Add(InventorySlotData.NullSlotData());
                continue;
            }
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

    public bool IsOpen()
    {
        return inventoryRoot.gameObject.activeSelf;
    }
}
