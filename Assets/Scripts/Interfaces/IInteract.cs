using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteract
{
    void Select();
    void Attack();
    void Examine();
    void Disarm();
    void Drop();
    void Pickup();
    void Push();
    void Unlock();
    void Use();
    void Search();
    void Talk();
}
