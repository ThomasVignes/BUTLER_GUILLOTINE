using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildNavigator : MonoBehaviour
{
    PersistentData persistentData;

    public void Init(PersistentData persistentdata)
    {
        this.persistentData = persistentdata;
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue()
    {
        if (!CanContinue())
            NextScene();

        SceneManager.LoadScene(persistentData.CurrentScene);
    }

    public bool CanContinue()
    {
        return persistentData.CurrentScene != "";
    }

    public void RequestQuit()
    {
        persistentData.SaveData();

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
