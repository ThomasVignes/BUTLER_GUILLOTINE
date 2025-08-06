using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseCameraFollows : MonoBehaviour
{        
    #region Variables
    [Header("Settings outside")]
    [SerializeField] private float _outsideMaxRotationX; 
    [SerializeField] private float _outsideMaxRotationY;
    
    [Header("Settings inside")]
    [SerializeField] private float _insideMaxRotationX; 
    [SerializeField] private float _insideMaxRotationY;

    private bool _hasMouseMoved = false;
    private Vector3 _startRotation;
    private float _targetXRotation;
    private float _targetYRotation;
    private KeyboardZoneManager _zoneManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _startRotation = transform.localEulerAngles;
        _zoneManager = KeyboardZoneManager.Instance;
    }
    private void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) _hasMouseMoved = true;
        if (!_hasMouseMoved) return; 
        Vector2 mousePos = Input.mousePosition; 
        float screenWidth = Screen.width; 
        float screenHeight = Screen.height;
        
        float normalizedX = (mousePos.x / screenWidth - 0.5f) * 2f;
        float normalizedY = (mousePos.y / screenHeight - 0.5f) * 2f;
        
        if (_zoneManager.IsCollider)
        { 
            SensitivityOfCamera(_insideMaxRotationX, _insideMaxRotationY,normalizedX,normalizedY);
        }
        else
        {
            SensitivityOfCamera(_outsideMaxRotationX, _outsideMaxRotationY,normalizedX,normalizedY);
        }
        transform.localRotation = Quaternion.Euler(_targetXRotation, _targetYRotation, _startRotation.z);
    }
    #endregion
    
    #region Private Methods
    private void SensitivityOfCamera(float maxRotationX, float maxRotationY, float normalizedX, float normalizedY)
    {
        _targetXRotation = -normalizedY * maxRotationX + _startRotation.x;
        _targetYRotation = normalizedX * maxRotationY + _startRotation.y;
    }
    #endregion
}