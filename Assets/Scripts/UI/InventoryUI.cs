using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Button hideInventoryButton;
    [SerializeField] Button showInventoryButton;

    private void Awake()
    {
        hideInventoryButton.onClick.AddListener(()=> {
            GameManager.Instance.ClosePlayerInventoryUI();
        });
        showInventoryButton.onClick.AddListener(() => {
            GameManager.Instance.OpenPlayerInventoryUI();
        });
    }

}
