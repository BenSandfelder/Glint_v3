using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalMessage : MonoBehaviour
{
    [SerializeField]
    private string header;
    [SerializeField]
    [TextArea(2, 4)]
    private string body;

    public void GetNewMessage()
    {
        ModalManager.instance.ShowModal(header, body);
    }
}
