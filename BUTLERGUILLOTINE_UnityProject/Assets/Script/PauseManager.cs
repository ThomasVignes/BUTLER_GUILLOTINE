using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : Manager
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] Slider masterVolume;
    bool canPause;

    float baseScale;

    public bool CanPause { get { return canPause; } set { canPause = value; } }


    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        baseScale = Time.timeScale;

        canPause = true;

        if (PersistentData.Instance != null)
            masterVolume.value = PersistentData.Instance.MasterVolume;
    }

    public override void Step()
    {
        if (!canPause)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            gm.Paused = !gm.Paused;

            pauseUI.SetActive(gm.Paused);

            if (gm.Paused)
                Time.timeScale = 0;
            else
                Time.timeScale = baseScale;
        }
    }

    public void UnPause()
    {
        gm.Paused = false;
        pauseUI.SetActive(false);

        Time.timeScale = baseScale;
    }

    public void UpdateVolume()
    {
        if (PersistentData.Instance != null)
            PersistentData.Instance.MasterVolume = masterVolume.value;
    }

    public void MainMenu()
    {
        UnPause();

        Cursor.visible = true;

        SceneManager.LoadScene("NewMenu");
    }

    public void QuitGame()
    {
        UnPause();

        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
