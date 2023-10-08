using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.WorldSelectScene);
        });
        optionsButton.onClick.AddListener(() =>
        {

        });
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }


}
