using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bai : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Baiii()
    {
        Application.Quit();
    }
}
