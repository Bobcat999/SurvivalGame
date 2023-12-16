using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerHandManager : MonoBehaviour
{
    public static PlayerHandManager Instance;
    public PlayerControls controls;
    public GameObject lootPrefab;
    public GameObject inventoryItemPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        controls = new PlayerControls();
        controls.Inventory.Enable();
        controls.Inventory.Click.performed += Click_performed;
        controls.Inventory.AltClick.performed += AltClick_performed; ;
        controls.Inventory.DropItem.performed += DropItem_performed;
    }


    private void AltClick_performed(InputAction.CallbackContext obj)
    {
        // Create a pointer event data
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

        // Set the position of the pointer to the mouse position
        pointerEventData.position = Mouse.current.position.value;

        // Create a list to store the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast into the scene using the EventSystem
        EventSystem.current.RaycastAll(pointerEventData, results);

        // Check if any UI elements were hit
        if (results.Count > 0 && results.FindAll((RaycastResult r) => { return r.gameObject.GetComponent<InventorySlot>() != null; }).Count > 0)
        {
            // Get the first UI element hit
            GameObject hitObject = results.FindAll((RaycastResult r) => { return r.gameObject.GetComponent<InventorySlot>() != null; })[0].gameObject;
            InventorySlot slot = hitObject.GetComponent<InventorySlot>();

            // Check if the UI element has a certain component
            if (slot != null)
            {
                OnAltClickedSlot(hitObject.GetComponent<InventorySlot>());
            }
        }
    }

    private void OnDestroy()
    {
        controls.Inventory.Click.performed -= Click_performed;
        controls.Inventory.DropItem.performed -= DropItem_performed;

    }

    private void Click_performed(InputAction.CallbackContext obj)
    {
        // Create a pointer event data
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

        // Set the position of the pointer to the mouse position
        pointerEventData.position = Mouse.current.position.value;

        // Create a list to store the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast into the scene using the EventSystem
        EventSystem.current.RaycastAll(pointerEventData, results);

        // Check if any UI elements were hit
        if (results.Count > 0 && results.FindAll((RaycastResult r) => { return r.gameObject.GetComponent<InventorySlot>() != null; }).Count > 0)
        {
            // Get the first UI element hit
            GameObject hitObject = results.FindAll((RaycastResult r) => { return r.gameObject.GetComponent<InventorySlot>() != null; })[0].gameObject;
            InventorySlot slot = hitObject.GetComponent<InventorySlot>();

            // Check if the UI element has a certain component
            if (slot != null)
            {
                OnClickedSlot(hitObject.GetComponent<InventorySlot>());
            }
        }
        else if (GetHandItem(false) != null && GetHandItem(false).type == ItemType.Item)//if we didnt hit and we are holding an item anything then just drop it
        {
            DropCurrentItem();
        }
    }

    private void DropItem_performed(InputAction.CallbackContext obj)
    {
        DropCurrentItem();
    }

    private void Update()
    {
        //set the position as the mouse position
        transform.position = Mouse.current.position.value;

        //hotswap with a slot based on input
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 10)
            {
                OnClickedSlot(GameManager.Instance.playerInventory.GetInventorySlots()[number - 1/* minus 1 because slots start at 0 and input starts at 1*/]);
            }
        }
    }

    public void OnClickedSlot(InventorySlot slot)
    {
        InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();//get the other item
        InventoryItem handItem = transform.GetComponentInChildren<InventoryItem>();

        if (slotItem == null || handItem == null || slotItem.item != handItem.item)//if the item in our hand and slot dont match then just switch them
        {
            if (transform.childCount > 0)//if we have an item then put it in the other slot
            {
                if (!slot.canHandPlace)//if we have an item and we can't place an item then return
                {
                    return;
                }
                transform.GetChild(0).SetParent(slot.transform);
                Debug.Log("Item placed in: " + slot.name);
            }
            if (slotItem != null)//if the slot had an item then put it our hand
            {
                slotItem.transform.SetParent(transform);
            }
        }
        else//otherwise place the items from our hand into the slot
        {
            slotItem.count += handItem.count;
            Debug.Log("Added stacks: " + slotItem.count);
            if (slotItem.count > Inventory.ITEM_STACK_COUNT)
            {
                handItem.count = slotItem.count - Inventory.ITEM_STACK_COUNT;
            }
            else
            {
                Destroy(handItem.gameObject);

                Debug.Log("Destroyed other stack: " + handItem.count);
            }
            slotItem.RefreshCount();
        }
        //notify the inventory
        Inventory.InventoryChanged();
    }

    public void OnAltClickedSlot(InventorySlot slot)
    {
        InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();//get the other item
        InventoryItem handItem = transform.GetComponentInChildren<InventoryItem>();
        if (slotItem == null && handItem != null)
        {
            slotItem = SpawnNewItem(handItem.item);
            slotItem.transform.SetParent(slot.transform);
            slotItem.count = 1;
            slotItem.RefreshCount();

            handItem.count--;
            handItem.RefreshCount();

            if (handItem.count == 0)
            {
                Destroy(handItem.gameObject);
            }
        }
        else if (slotItem != null && handItem == null && slotItem.count > 1)
        {
            handItem = SpawnNewItem(slotItem.item);
            handItem.transform.SetParent(transform);
            handItem.count = slotItem.count / 2;
            handItem.RefreshCount();

            slotItem.count = (slotItem.count % 2 == 0) ? slotItem.count / 2 : slotItem.count / 2 + 1;
            slotItem.RefreshCount();

        }
    }

    public void DropCurrentItem()
    {
        if (transform.childCount > 0)
        {
            InventoryItem item = transform.GetComponentInChildren<InventoryItem>();
            Vector3 lootSpawnPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            lootSpawnPos.z = 0;//make sure it renderes with everything else
            GameObject loot = Instantiate(lootPrefab, lootSpawnPos, Quaternion.identity);
            loot.GetComponent<Loot>().Initialize(item.item, item.count);

            //destroy the current item
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public InventorySlotData GetHandData()
    {
        InventoryItem item = GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            return new InventorySlotData(item.item.id, item.count);
        }
        else
        {
            return InventorySlotData.NullSlotData();
        }
    }

    public Item GetHandItem(bool use)
    {
        InventoryItem itemInSlot = GetComponentInChildren<InventoryItem>();
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
    }

    public void LoadHandData(InventorySlotData slotData)
    {
        //destroy current item if there is one
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        //spawns the new item and sets the count
        if (WorldManager.Instance.GetItemFromItemId(slotData.itemId) != default)
        {
            SpawnNewItem(WorldManager.Instance.GetItemFromItemId(slotData.itemId)).count = slotData.count;
        }
    }

    InventoryItem SpawnNewItem(Item item)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
        return inventoryItem;
    }
}
