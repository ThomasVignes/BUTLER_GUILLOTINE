using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildNavigator : MonoBehaviour
{
    PersistentData persistentData;

    bool menuRequested, quitRequested;

    public void Init(PersistentData persistentdata)
    {
        this.persistentData = persistentdata;
    }

    public void NextScene()
    {
        if (persistentData.DemoMode && persistentData.HasKey)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NoSaveToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RequestMenu()
    {
        if (menuRequested)
            return;

        menuRequested = true;

        persistentData.SaveData();

        StartCoroutine(C_MenuDelay());
    }

    IEnumerator C_MenuDelay()
    {
        yield return new WaitForSeconds(1f);

        menuRequested = false;
        SceneManager.LoadScene(0);
    }

    public void Continue()
    {
        if (!CanContinue())
            NextScene();

        SceneManager.LoadScene(persistentData.CurrentSceneIndex);
    }

    public bool CanContinue()
    {
        return persistentData.CurrentSceneIndex > 0;
    }

    public void RequestQuit()
    {
        if (quitRequested)
            return;

        quitRequested = true;

        persistentData.SaveData(false);

        StartCoroutine(C_QuitDelay());
    }

    IEnumerator C_QuitDelay()
    {
        yield return new WaitForSeconds(1);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
