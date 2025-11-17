using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bai : MonoBehaviour
{
    [SerializeField] string achievement;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        TryToUpdateData();
    }

    void TryToUpdateData()
    {
        if (PersistentData.Instance != null)
        {
            if (!PersistentData.Instance.FinishedOnce)
            {
                PersistentData.Instance.FinishedOnce = true;
                PersistentData.Instance.SaveData();
            }

            PersistentData.Instance.SteamAchievementManager.TriggerAchievement(achievement, true);

        }
    }

    public void Baiii()
    {
        Cursor.visible = true;

        ButlerEngineUtilities.ClearAllInterScenes();

        SceneManager.LoadScene(0);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
