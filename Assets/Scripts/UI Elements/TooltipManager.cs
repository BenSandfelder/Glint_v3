using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public RectTransform tipWindow;

    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;
    
    private void OnEnable()
    {
        //Shorthand for registering the events.
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }

    private void OnDisable()
    {
        //Shorthand for unregistering the events.
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }

    void Start()
    {
        tipText.text = "";
        tipWindow.gameObject.SetActive(false);
    }
    private void ShowTip(string tip, Vector2 mousePos)
    {
        tipText.text = tip;
        //Turnary Operator - if statement inside a statement. If TipText.preferred width > 80, use 80. Else, use the preferred width.
        tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > 80 ? 80 : tipText.preferredWidth, tipText.preferredHeight);

        tipWindow.gameObject.SetActive(true);
        tipWindow.transform.position = new Vector2(mousePos.x + 8, mousePos.y);
    }

    public void HideTip()
    {
        tipText.text = "";
        tipWindow.gameObject.SetActive(false);
    }
}
