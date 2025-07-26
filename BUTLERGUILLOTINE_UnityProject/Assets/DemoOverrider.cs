using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoOverrider : MonoBehaviour
{
    public bool Active;
    public string EndScene;
    public GameObject CinematicThing;


    private void Awake()
    {
        if (PersistentData.Instance != null) 
        {
            if (PersistentData.Instance.DemoMode)
                Active = true;

        }
    }

    public void Part1()
    {
        if (!Active)
            return;

        GameManager.Instance.ThemeManager.CreateInterScene("MeinHerze2");
    }

    public void Part2()
    {
        if (!Active)
            return;

        CinematicThing.SetActive(false);
        GameManager.Instance.End = true;
        StartCoroutine(C_EndScene());
    }

    IEnumerator C_EndScene()
    {
        GameManager.Instance.ScreenEffects.FadeTo(1, 1);

        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene(EndScene);
    }
}
