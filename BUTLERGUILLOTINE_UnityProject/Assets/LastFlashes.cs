using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFlashes : MonoBehaviour
{
    [SerializeField] ScreenEffects screenEffects;
    [SerializeField] GameObject[] cams;
    [SerializeField] float camDuration, flashDuration;

    int index = -1;

    private void Start()
    {
        NextCam();
    }

    void NextCam()
    {
        if (index < cams.Length - 1)
        {
            screenEffects.WhiteFlash();
            EffectsManager.Instance.audioManager.Play("Flash");

            if (index >= 0)
                cams[index].gameObject.SetActive(false);

            index++;

            cams[index].gameObject.SetActive(true);

            StartCoroutine(C_WaitForNext());
        }
        else
            PersistentData.Instance.BuildNavigator.NextScene();
    }

    IEnumerator C_WaitForNext()
    {
        yield return new WaitForSeconds(camDuration);

        NextCam();
    }
}
