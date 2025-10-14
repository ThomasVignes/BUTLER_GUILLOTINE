using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildNavigator : MonoBehaviour
{
    PersistentData persistentData;

    public void Init(PersistentData persistentdata)
    {
        this.persistentData = persistentdata;
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
