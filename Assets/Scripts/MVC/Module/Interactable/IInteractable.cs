using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string Text { get; }

    bool CanInteract {  get; }

    void Interact();
}
