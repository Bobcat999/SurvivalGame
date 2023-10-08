using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSingleUI : MonoBehaviour
{

    string worldName;

    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI nameText;

    private void Awake()
    {
        button.onClick.AddListener(() =>
        {
            WorldManager.SetLoadCommand(new LoadWorldCommand(worldName));
            Loader.Load(Loader.Scene.GameScene);
        });
    }

    public void SetWorld(string worldName)
    {
        this.worldName = worldName;
        nameText.text = worldName;
    }


}
