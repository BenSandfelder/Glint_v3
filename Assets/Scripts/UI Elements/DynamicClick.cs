using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private AdvancedCustomEvent LeftClick;
    [SerializeField]
    private AdvancedCustomEvent RightClick;
    [SerializeField]
    private AdvancedCustomEvent PointerHover;
    [SerializeField]
    private AdvancedCustomEvent PointerExit;

    // Start is called before the first frame update
    private void Awake()
    {

    }

    //On a complete click (down + up).
    public void OnPointerClick(PointerEventData eventdata)
    {
        //On a left click...
        if (eventdata.button == PointerEventData.InputButton.Left)
        {
            LeftClick.Invoke(this, this);
        }
        //On a right click...
        else if (eventdata.button == PointerEventData.InputButton.Right)
        {
            RightClick.Invoke(this, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventdata)
    {
        PointerHover.Invoke(this, this);
    }

    public void OnPointerExit(PointerEventData eventdata)
    {
        PointerHover.Invoke(this, this);
    }
}
