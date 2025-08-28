using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButlerEngineUtilities : MonoBehaviour 
{
    public static void ClearAllInterScenes()
    {
        var interScenes = FindObjectsOfType<InterSceneTheme>();
        for (int i = 0; i < interScenes.Length; i++)
        {
            interScenes[i].Stop();
            Destroy(interScenes[i].gameObject);
        }
    }
}
