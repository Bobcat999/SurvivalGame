using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{

    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;

    public LayoutElement layoutElement;

    public int charectarWrapLimit;

    public Vector2 offset;



    public void SetText(string header, string content = "")
    {
        if (string.IsNullOrEmpty(content))
        {
            contentText.gameObject.SetActive(false);
        }
        else
        {
            contentText.gameObject.SetActive(true);
            contentText.text = content;
        }

        headerText.text = header;

        int headerTextLength = headerText.text.Length;
        int contentTextLength = contentText.text.Length;

        layoutElement.enabled = (headerTextLength > charectarWrapLimit || contentTextLength > charectarWrapLimit) ? true : false;

    }

    public void Update()
    {
        if (Application.isEditor)
        {
            int headerTextLength = headerText.text.Length;
            int contentTextLength = contentText.text.Length;

            layoutElement.enabled = (headerTextLength > charectarWrapLimit || contentTextLength > charectarWrapLimit) ? true : false;
        }

        Vector2 mousePos = Mouse.current.position.value;

        transform.position = mousePos + offset;

    }

}
