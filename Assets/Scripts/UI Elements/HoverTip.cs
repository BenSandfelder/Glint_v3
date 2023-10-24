using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tipToShow;
    [SerializeField]
    private float timeToWait = 1f;

    public void OnPointerEnter(PointerEventData data)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("Exited");
        StopAllCoroutines();
        TooltipManager.OnMouseLoseFocus();
    }

    private void ShowTooltip()
    {
        TooltipManager.OnMouseHover(tipToShow, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowTooltip();
    }
}
