using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsLocalManager : MonoBehaviour
{
    [SerializeField] Achievement[] achievements;

    public void TriggerAchievement(string ID)
    {
        Achievement achievement = Array.Find(achievements, achiev => achiev.ID == ID);

        if (achievement == null)
            return;

        if (!achievement.Implemented)
            return;

        if (achievement.Increments > 0)
            achievement.Increments--;
        else
        {
            PersistentData.Instance.SteamAchievementManager.TriggerAchievement(achievement.ID);
            Debug.Log("Triggered achievement: " +  achievement.ID);
        }
    }

    public void FailAchievement(string ID)
    {
        Achievement achievement = Array.Find(achievements, achiev => achiev.ID == ID);

        if (achievement == null)
            return;

        if (!achievement.Implemented)
            return;

        achievement.Implemented = false;

        Debug.Log("Failed achievement: " + achievement.ID);
    }
}

[System.Serializable]
public class Achievement
{
    public string ID;
    public int Increments;
    public bool Implemented;
}
