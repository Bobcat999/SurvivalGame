using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldEditUI : MonoBehaviour
{

    [SerializeField] TMP_InputField newWorldNameInputField;
    [SerializeField] Button saveChangesButton;
    [SerializeField] Button deleteWorldButton;
    [SerializeField] Button closeButton;

    string worldFile;

    private void Awake()
    {
        saveChangesButton.onClick.AddListener(() =>
        {
            //change the file name
            File.Move(worldFile, Path.Combine(Path.GetDirectoryName(worldFile), newWorldNameInputField.text + ".json"));
            Loader.Load(Loader.Scene.WorldSelectScene);
        });
        deleteWorldButton.onClick.AddListener(() =>
        {
            File.Delete(worldFile);
            Loader.Load(Loader.Scene.WorldSelectScene);
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        Hide();
    }


    public void EditWorldFile(string filename)
    {
        worldFile = filename;
        newWorldNameInputField.text = Path.GetFileNameWithoutExtension(filename);
        Show();
    }



    void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

}
