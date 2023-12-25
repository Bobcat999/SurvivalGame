using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour //, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;

    public TooltipTrigger tooltip;


    [HideInInspector] public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    void Start()
    {
        InitializeItem(item);
    }


    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;

        //setup the tooltip
        tooltip.header = item.name;
        switch (item.type)
        {
            case ItemType.Tool:
                tooltip.content = "Breaking Speed: " + item.breakingSpeed + "\n" + item.description;
                break;
            default:
                tooltip.content = item.description;
                break;
        }

        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }


    /*public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        countText.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Mouse.current.position.x.value, Mouse.current.position.y.value, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        countText.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }*/
}
