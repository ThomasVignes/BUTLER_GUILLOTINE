using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableInteractable : Interactable
{
    private void OnMouseDown()
    {
        Interact();
    }
}
