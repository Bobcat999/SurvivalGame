using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI loadingText;

    private void Awake()
    {
        WorldManager.OnLoadingStarted += WorldManager_OnLoadingStarted;
        WorldManager.OnLoadingEnded += WorldManager_OnLoadingEnded;
        WorldManager.OnCreateWorldStarted += WorldManager_OnCreateWorldStarted;
        WorldManager.OnCreateWorldEnded += WorldManager_OnCreateWorldEnded;
    }

    private IEnumerator AnimateText(string text)
    {
        int numDots = 0;
        Debug.Log("anim started");

        while (isActiveAndEnabled)
        {
            string dots = "";
            for (int i = 0; i < numDots; i++)
            {
                dots += ".";
            }
            loadingText.text = text + dots;
            numDots = (numDots + 1) % 4; // Use modulo to cycle through 0, 1, 2, 3
            Debug.Log(numDots);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void WorldManager_OnCreateWorldEnded()
    {
        Hide();
    }

    private void WorldManager_OnCreateWorldStarted()
    {
        Show();
        StartCoroutine(AnimateText("Creating World"));
    }

    private void WorldManager_OnLoadingEnded()
    {
        Hide();
    }

    private void WorldManager_OnLoadingStarted()
    {
        Show();
        StartCoroutine(AnimateText("Loading"));
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        WorldManager.OnLoadingStarted -= WorldManager_OnLoadingStarted;
        WorldManager.OnLoadingEnded -= WorldManager_OnLoadingEnded;
        WorldManager.OnCreateWorldStarted -= WorldManager_OnCreateWorldStarted;
        WorldManager.OnCreateWorldEnded -= WorldManager_OnCreateWorldEnded;

    }
}
