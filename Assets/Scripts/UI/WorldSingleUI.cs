using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSingleUI : MonoBehaviour
{

    string worldName;
    string filePath;

    [SerializeField] Button playButton;
    [SerializeField] Button editButton;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] WorldEditUI worldEditUi;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            WorldManager.SetLoadCommand(new LoadWorldCommand(worldName));
            Loader.Load(Loader.Scene.GameScene);
        });
        editButton.onClick.AddListener(() =>
        {
            worldEditUi.EditWorldFile(filePath);
        });
    }

    public void SetWorld(string worldName, string filePath)
    {
        this.worldName = worldName;
        this.filePath = filePath;
        nameText.text = worldName;
    }


}
