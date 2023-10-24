using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{
    public static FollowMouse instance;
    private Sprite defaultPointer;
    private Color defaultColor = Color.white; 
    private Image currentPointer;

    void Start()
    {
        currentPointer = GetComponent<Image>();
        defaultPointer = currentPointer.sprite;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void UpdatePointer(Sprite newpointer, Color tint)
    {
        currentPointer.sprite = newpointer;
        currentPointer.color = tint;
    }

    public void ResetPointer()
    {
        currentPointer.sprite = defaultPointer;
        currentPointer.color = Color.white;
    }
}
