using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTutorial : Tutorial
{
    private void Update()
    {
        if (!active)
            return;


        if (Input.GetButtonDown("Pause"))
        {
            EndTutorial();
        }
    }
}
