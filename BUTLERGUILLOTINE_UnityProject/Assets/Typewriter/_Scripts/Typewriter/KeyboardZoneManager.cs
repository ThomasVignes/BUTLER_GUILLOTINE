using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class KeyboardZoneManager : MonoBehaviour
{
    #region Variables
    public static KeyboardZoneManager Instance; 
    
    [HideInInspector] public bool IsCollider = false; 
    
    [Header("References")]
    public GameObject zoneClickableObject;
    [SerializeField] private GameObject _player;

    [Header("Settings Enter")]
    [HideInInspector] public  Vector3 EnterPosition;
    [HideInInspector] public Vector3 EnterRotation;
    [SerializeField] private GameObject _enterPositionAndRotation;
    
    [Header("Settings Exit")]
    [HideInInspector] public Vector3 ExitPosition;
    [HideInInspector] public Vector3 ExitRotation;
    [SerializeField] private GameObject _exitPositionAndRotation;
    
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        EnterPosition = _enterPositionAndRotation.transform.position;
        EnterRotation = _enterPositionAndRotation.transform.eulerAngles;
        ExitPosition = _exitPositionAndRotation.transform.position;
        ExitRotation = _exitPositionAndRotation.transform.eulerAngles;
    }
    #endregion
    
    #region Public Methods
    public void ChangeDirection(bool isCollider, Vector3 position, Vector3 rotation)
    {
        IsCollider = isCollider;

        Vector3 currentPosition = _player.transform.position;
        Vector3 currentRotation = _player.transform.eulerAngles;

        Vector3 newPosition = new Vector3(position.x, currentPosition.y, position.z);
        Vector3 newRotation = new Vector3(rotation.x, currentRotation.y, rotation.z);


        // plus propre que :
        // CharacterController characterController = _player.GetComponent<CharacterController>();
        // if (characterController != null)
        if (_player.TryGetComponent<CharacterController>(out var characterController))
        {
            characterController.enabled = false;
            _player.transform.position = newPosition;
            _player.transform.rotation = Quaternion.Euler(newRotation);
            characterController.enabled = true;
        }
        else
        {
            _player.transform.position = newPosition;
            _player.transform.rotation = Quaternion.Euler(newRotation);
        }
    }
    #endregion
    
    #region Private Methods
    private void OnMouseDown()
    {
        TypewriterPlayer.Instance.GoToKeyboard();
    }
    #endregion
}