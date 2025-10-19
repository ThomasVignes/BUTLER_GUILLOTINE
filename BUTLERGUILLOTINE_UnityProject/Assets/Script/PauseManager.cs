using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : Manager
{
    [SerializeField] SettingsManager settingsManager;
    [SerializeField] GameObject pauseUI, optionsUI;
    bool canPause, optionsOpen;

    float baseScale;

    public bool CanPause { get { return canPause; } set { canPause = value; } }


    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        baseScale = Time.timeScale;

        canPause = true;

        settingsManager.Init();
    }

    public override void Step()
    {
        if (!canPause)
            return;
        
        
        if (Input.GetButtonDown("PauseGame")) 
        { 
            gm.Paused = !gm.Paused;

            pauseUI.SetActive(gm.Paused);

            if (gm.Paused)
                Time.timeScale = 0;
            else
                Time.timeScale = baseScale;
        }
        
    }

    public void TogglePause(bool paused)
    {
        gm.Paused = paused;

        pauseUI.SetActive(gm.Paused);

        if (gm.Paused)
            Time.timeScale = 0;
        else
            Time.timeScale = baseScale;

        if (!gm.Paused)
        {
            optionsOpen = false;
            optionsUI.SetActive(optionsOpen);
        }
    }

    public void UnPause()
    {
        TogglePause(false);
    }

    public void ToggleOptions()
    {
        optionsOpen = !optionsOpen;
        optionsUI.SetActive(optionsOpen);
    }

    public void MainMenu()
    {
        UnPause();

        Cursor.visible = true;

        ButlerEngineUtilities.ClearAllInterScenes();

        PersistentData.Instance.BuildNavigator.RequestMenu();
    }

    public void QuitGame()
    {
        UnPause();

        PersistentData.Instance.BuildNavigator.RequestQuit();
    }
}
