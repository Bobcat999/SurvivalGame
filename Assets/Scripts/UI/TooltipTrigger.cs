using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string header;
    [Multiline()]
    public string content;

    private static LTDescr delay;

    bool isHovered = false;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        delay = LeanTween.delayedCall(0.5f, () =>
        {
            TooltipSystem.Show(header, content);
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        isHovered = false;
        TooltipSystem.Hide();
    }

    // Call this method when you want to manually trigger the exit event
    public void ForceTooltipHide()
    {
        if (isHovered)
        {
            isHovered = false;
            OnPointerExit(null); // Pass null or create a dummy PointerEventData if needed
        }
    }

    private void OnDisable()
    {
        ForceTooltipHide();
    }

}
