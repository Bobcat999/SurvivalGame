using System.Collections;
using UnityEngine;

public class PlayerInventory : Inventory
{

    /*int selectedSlot = 0;

    public void SetSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public void ChangeSelectedSlot(int change)
    {
        int newSlot = selectedSlot + change;
        newSlot = (newSlot >= SLOTS_PER_ROW) ? newSlot - SLOTS_PER_ROW : (newSlot < 0) ? newSlot + SLOTS_PER_ROW : newSlot;
        SetSelectedSlot(newSlot);
    }


    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;

            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }

            return item;
        }

        return null;
    }*/

    
}