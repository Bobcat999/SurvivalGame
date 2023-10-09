using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WorldSelectUI : MonoBehaviour
{
    [SerializeField] private Button newWorldButton;
    [SerializeField] private Transform worldCreateUI;

    [SerializeField] private Transform worldsHolder;
    [SerializeField] private Transform worldSingleUIPrefab;

    private void Awake()
    {
        newWorldButton.onClick.AddListener(() => {
            worldCreateUI.gameObject.SetActive(true);
        });
    }

    public void Start()
    {
        worldSingleUIPrefab.gameObject.SetActive(false);
        RefreshWorlds();
    }


    public void RefreshWorlds()
    {
        WorldManager.SetUpWorldsDirectory();

        string[] worlds = Directory.GetFiles(WorldManager.GetWorldsDirectory());

        foreach(Transform child in worldsHolder)
        {
            if(child != worldSingleUIPrefab)
            {
                Destroy(child);
            }
        }


        foreach (string s in worlds)
        {
            Transform worldTransform = Instantiate(worldSingleUIPrefab, worldsHolder);
            worldTransform.gameObject.SetActive(true);

            worldTransform.GetComponent<WorldSingleUI>().SetWorld(Path.GetFileNameWithoutExtension(s));
        }

    }



}
