using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampfireInventoryUI : MonoBehaviour
{

    [SerializeField] Slider smealtingProgressSlider;
    [SerializeField] Image fireImage;

    [SerializeField] Sprite flameSprite;
    [SerializeField] Sprite noFlameSprite;

    public void SetFlame(bool hasFlame)
    {
        fireImage.sprite = hasFlame ? flameSprite : noFlameSprite;
    }

    public void SetProgress(float progress)
    {
        smealtingProgressSlider.gameObject.SetActive(progress > 0);
        smealtingProgressSlider.value = progress;
    }



}
