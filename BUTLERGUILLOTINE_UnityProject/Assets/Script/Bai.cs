using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bai : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Baiii()
    {
        Cursor.visible = true;

        ButlerEngineUtilities.ClearAllInterScenes();

        SceneManager.LoadScene(1);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
