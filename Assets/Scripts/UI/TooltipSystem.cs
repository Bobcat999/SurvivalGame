using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{

    private static TooltipSystem Instance;

    public Tooltip tooltip;

    public void Awake()
    {
        Instance = this; 
    }

    public static void Show(string header, string content = "")
    {
        Instance.tooltip.SetText(header, content);
        Instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide() 
    {
        Instance.tooltip.gameObject.SetActive(false);
    }


}
