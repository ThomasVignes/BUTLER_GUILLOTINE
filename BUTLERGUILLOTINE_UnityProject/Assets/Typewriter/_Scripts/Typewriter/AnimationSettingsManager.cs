using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSettingsManager : MonoBehaviour
{
    #region Variables
    public static AnimationSettingsManager Instance;
    [Header("All Settings")] 
    public float DownDistance;
    public float Speed;
    public float SoftPress; 
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void EnableArms(bool arms)
    {
        TypewriterPlayer.Instance.SetIK(arms);
    }
    #endregion
}
