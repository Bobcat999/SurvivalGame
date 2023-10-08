using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldCreateUI : MonoBehaviour
{

    [SerializeField] private Button closeButton;
    [SerializeField] private Button createNewWorldButton;
    [SerializeField] private TMP_InputField worldNameInputField;
    [SerializeField] private TMP_InputField worldSeedInputField;


    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        createNewWorldButton.onClick.AddListener(() =>
        {
            WorldManager.SetLoadCommand(new CreateNewWorldCommand(worldNameInputField.text, int.Parse(worldSeedInputField.text)));
            Loader.Load(Loader.Scene.GameScene);
        });

        gameObject.SetActive(false);
    }


}
