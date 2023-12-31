using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour/*, IDropHandler*/
{

    public Image image;
    public Color selectedColor, notSelectedColor;
    public bool canHandPlace = true;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public bool HasItem()
    {
        return transform.childCount > 0;
    }
/*
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerHandManager.Instance.OnClickedSlot(this);
    }*/


    /*public void OnDrop(PointerEventData eventData)
    {
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (transform.childCount > 0)
        {
            transform.GetChild(0).SetParent(inventoryItem.parentAfterDrag);
        }
        inventoryItem.parentAfterDrag = transform;
        //notify the inventory
        Inventory.InventoryChanged();
    }*/
}
