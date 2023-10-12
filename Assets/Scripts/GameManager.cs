using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public PlayerControls controls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

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
        playerInventory.SetSelectedSlot(0);
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

    #region Chests
    [Header("Chests")]
    public List<ChestInventory> chestInventories = new List<ChestInventory>();
    public Transform chestInventoryUIParent;
    public GameObject chestInventoryUIPrefab;
    public GameObject lootPrefab;


    private void Interact_performed(InputAction.CallbackContext context)
    {

        Vector3 coor = Mouse.current.position.ReadValue();
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(coor), Vector2.zero);
        if (hit && hit.collider.GetComponent<ChestInventory>() != null)
        {
            ChestInventory clickedChest = hit.collider.GetComponent<ChestInventory>();
            Debug.Log("hit: " + hit.collider.transform.name);
            //open the chest inventory and the player inventory
            clickedChest.OpenInventory();
            playerInventory.OpenInventory();
        }
    }

    public void SetupChest(ChestInventory chestInventory)
    {
        if (chestInventories.Contains(chestInventory))
            return;

        chestInventories.Add(chestInventory);

        //spawn chest inventory object
        GameObject inventoryObject = Instantiate(chestInventoryUIPrefab, chestInventoryUIParent);
        chestInventory.inventoryRoot = inventoryObject.transform;
        //get the slots
        InventorySlot[] slots = inventoryObject.GetComponentsInChildren<InventorySlot>();
        //set the chest objects slots
        chestInventory.SetInventorySlots(slots);
        //close the chest
        chestInventory.CloseInventory();
    }

    public void RemoveChest(ChestInventory chestInventory)
    {
        chestInventories.Remove(chestInventory);

        //destroy the ui object
        if (chestInventory.inventoryRoot)
        {
            Destroy(chestInventory.inventoryRoot.gameObject);
        }
    }

    #endregion

    #region Inventory
    [Header("Inventory")]
    public PlayerInventory playerInventory;
    public Item[] startItems;
    public void Open_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //toggle the player inventory
        playerInventory.ToggleInventory();

        //close all of the chest inventories
        foreach (ChestInventory chest in chestInventories)
        {
            chest.CloseInventory();
        }
    }

    private void Scroll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float scroll = obj.ReadValue<float>();
        int numScroll = (scroll > 0) ? -1 : 1;
        playerInventory.ChangeSelectedSlot(numScroll);
    }
    #endregion

}