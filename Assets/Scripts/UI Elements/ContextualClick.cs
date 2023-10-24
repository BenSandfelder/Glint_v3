using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ContextualClick : MonoBehaviour, IPointerDownHandler
{
    //This script fires a different Custom Event based on whether the target is left-clicked or right-clicked.
    [SerializeField]
    private AdvancedCustomEvent LeftClick;
    [SerializeField]
    private AdvancedCustomEvent RightClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftClick.Invoke(this, 0);
        }

        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightClick.Invoke(this, 1);
        }
    }
}
