using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public PlayerControls controls;

    private void Awake()
    {
        Instance = this;

        //setup the controlls
        controls = new PlayerControls();
        controls.Enable();
        controls.Inventory.Scroll.performed += Scroll_performed;
        controls.Inventory.Open.performed += Open_performed;
        controls.Player.Interact.performed += Interact_performed;
    }

    private void OnDestroy()
    {
        controls.Dispose();
    }

    private void Start()
    {
        //select a slot in the players inventory
        playerInventory.SetSelectedSlot(0);
        //give player items to start with
        foreach (Item item in startItems)
        {
            playerInventory.AddItem(item);
        }

        playerInventory.CloseInventory();
    }

    public void Update()
    {
        //change the selected slot in a players inventory based on input
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 10)
            {
                playerInventory.SetSelectedSlot(number - 1);
            }
        }
    }

    #region Block Inventories
    [Header("Block Inentorys")]
    public List<BlockInventory> blockInventories = new List<BlockInventory>();
    public Transform blockInventoryUIParent;
    public GameObject lootPrefab;


    private void Interact_performed(InputAction.CallbackContext context)
    {

        Vector3 coor = Mouse.current.position.ReadValue();
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(coor), Vector2.zero);
        if (!IsInventoryOpen() && hit && hit.collider.GetComponent<BlockInventory>() != null)
        {
            BlockInventory clickedblock = hit.collider.GetComponent<BlockInventory>();

            //open the block inventory and the player inventory
            OpenInventoryUI(clickedblock.inventoryRoot);
        }
    }

    public void SetupBlockInventory(BlockInventory blockInventory)
    {
        if (blockInventories.Contains(blockInventory))
            return;

        blockInventories.Add(blockInventory);

        //spawn block inventory object
        GameObject inventoryObject = Instantiate(blockInventory.inventoryUIPrefab, blockInventoryUIParent);
        blockInventory.inventoryRoot = inventoryObject.transform;
        //get the slots
        InventorySlot[] slots = inventoryObject.GetComponentsInChildren<InventorySlot>();
        //set the block objects slots
        blockInventory.SetInventorySlots(slots);
        //close the block
        blockInventory.CloseInventory();
    }

    public void RemoveBlockInventory(BlockInventory blockInventory)
    {
        blockInventories.Remove(blockInventory);

        //destroy the ui object
        if (blockInventory.inventoryRoot)
        {
            Destroy(blockInventory.inventoryRoot.gameObject);
        }
    }

    #endregion

    #region Inventory
    [Header("Inventory")]
    public PlayerInventory playerInventory;
    public Transform craftingInventory;
    public Item[] startItems;

    public void OpenInventoryUI(Transform panel = null)
    {
        CloseInventoryUI();

        if (panel == null)
        {
            panel = craftingInventory;
        }

        //open the player inventory
        playerInventory.OpenInventory();

        //open the other inventory
        panel.gameObject.SetActive(true);

    }

    public void CloseInventoryUI()
    {
        //toggle the player inventory
        playerInventory.CloseInventory();

        //close all of the chest inventories
        foreach (BlockInventory blInv in blockInventories)
        {
            blInv.CloseInventory();
        }

        craftingInventory.gameObject.SetActive(false);

    }

    public bool IsInventoryOpen()
    {
        return playerInventory.IsOpen();
    }


    public void Open_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (IsInventoryOpen())
        {
            CloseInventoryUI();
        }
        else
        {
            OpenInventoryUI();
        }
    }

    private void Scroll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!IsInventoryOpen())
        {
            float scroll = obj.ReadValue<float>();
            int numScroll = (scroll > 0) ? -1 : 1;
            playerInventory.ChangeSelectedSlot(numScroll);
        }
    }
    #endregion

}