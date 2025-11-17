using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SteamAchievementManager : MonoBehaviour
{
    [SerializeField] SteamAchievement[] achievements;


    private void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.U))
        {
            bool done;
            SteamUserStats.GetAchievement("FIRSTDEATH", out done);
            Debug.Log("Achievement: " + done);

            if (!done)
            {
                SteamUserStats.SetAchievement("FIRSTDEATH");
                SteamUserStats.StoreStats();
                Debug.Log("SAVEDNEW?????????");
            }
        }
        */
    }

    public void TriggerAchievement(string Name, bool wee)
    {
        if (!SteamManager.Initialized)
            return;

        foreach (var item in achievements)
        {
            if (item.Name == Name)
            {
                bool done;
                SteamUserStats.GetAchievement(item.SteamID, out done);
                Debug.Log("Achievement: " + done);
                if (!done)
                { 
                    //SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
                    SteamUserStats.SetAchievement(item.SteamID);
                    SteamUserStats.StoreStats();
                }
                break;
            }
        }
    }
}

[System.Serializable]
class SteamAchievement
{
    public string Name;
    public string SteamID;
}
