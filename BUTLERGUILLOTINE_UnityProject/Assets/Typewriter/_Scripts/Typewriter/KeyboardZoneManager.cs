using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class KeyboardZoneManager : MonoBehaviour
{
    #region Variables
    [HideInInspector] public bool IsCollider = false;

    [Header("Settings Enter")]
    [HideInInspector] public  Vector3 EnterPosition;
    [HideInInspector] public Vector3 EnterRotation;
    [SerializeField] private GameObject _enterPositionAndRotation;
    public Vector2 CameraOffset;
    public Vector2 CameraConstraints;

    [Header("Events")]
    public UnityEvent OnReach;
    #endregion

    #region Unity Methods
    private void Start()
    {
        EnterPosition = _enterPositionAndRotation.transform.position;
        EnterRotation = _enterPositionAndRotation.transform.eulerAngles;
    }
    #endregion
    
    
    #region Private Methods
    private void OnMouseDown()
    {
        TypewriterPlayer.Instance.GoToKeyboard(this);
    }
    #endregion
}