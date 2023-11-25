using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakingProgressUI : MonoBehaviour
{
    [SerializeField] Slider progressSlider;


    private void Update()
    {
        if(BuildingSystem.Instance.currentBreakCommand == null)
        {
            progressSlider.gameObject.SetActive(false);
        }
        else
        {
            if(!progressSlider.gameObject.activeSelf)
            {
                progressSlider.gameObject.SetActive(true);
            }
            progressSlider.value = BuildingSystem.Instance.currentBreakCommand.GetProgress();
        }
    }
}
