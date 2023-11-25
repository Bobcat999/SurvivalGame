using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeItemSingleUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI countText;

    public TooltipTrigger tooltip;


    public void SetItemAndCount(Item item, int count)
    {
        image.sprite = item.image;
        countText.text = count.ToString();

        //setup the tooltip
        tooltip.header = item.name;
        tooltip.content = item.description;
    }
}
