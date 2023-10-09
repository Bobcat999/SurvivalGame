using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI: MonoBehaviour
{
    [SerializeField] Button optionsButton;//hook up options later
    [SerializeField] Button saveAndQuitButton;

    private PlayerControls playerControls;

    private void Awake()
    {
        saveAndQuitButton.onClick.AddListener(() =>
        {
            WorldManager.Instance.SaveCurrentWorld();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.UI.Menu.performed += Menu_performed;

        Hide();
    }

    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(gameObject.activeSelf)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void OnDestroy()
    {
        playerControls.Disable();
    }


    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

}
