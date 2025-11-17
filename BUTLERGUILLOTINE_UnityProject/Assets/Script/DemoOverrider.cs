using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class DemoOverrider : MonoBehaviour
{
    public bool Active;
    public string EndScene;
    public GameObject CinematicThing;

    bool ruth;

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



    public void SetAchievement()
    {
        if (!ruth)
            PersistentData.Instance.SteamAchievementManager.TriggerAchievement("FIRSTDEATH", true);
    }

    public void SetRuthAchievement()
    {
        if (ruth)
            PersistentData.Instance.SteamAchievementManager.TriggerAchievement("UNREACHABLE", true);
    }

    public void SetRuth(bool yes)
    {
        ruth = yes;
    }

    IEnumerator C_EndScene()
    {
        GameManager.Instance.ScreenEffects.FadeTo(1, 1);

        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene(EndScene);
    }
}
