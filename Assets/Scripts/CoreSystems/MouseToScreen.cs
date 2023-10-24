using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseToScreen : MonoBehaviour
{
    private static MouseToScreen instance;

    private void Awake()
    {
        instance = this;
    }
    // Update is called once per frame

    public static Vector3 GetPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}
