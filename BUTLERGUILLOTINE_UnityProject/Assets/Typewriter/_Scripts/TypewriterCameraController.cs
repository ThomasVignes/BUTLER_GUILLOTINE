using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TypewriterCameraController : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [SerializeField] float rotationThreshold;
    [SerializeField] float rotationSpeed;

    [Header("Settings outside")]
    [SerializeField] private float _outsideMaxRotationX; 
    [SerializeField] private float _outsideMaxRotationY;

    [Header("Settings inside")]
    [SerializeField] float xLookoffset;
    [SerializeField] float yLookoffset;
    [SerializeField] private float _insideMaxRotationX; 
    [SerializeField] private float _insideMaxRotationY;

    [Header("References")]
    [SerializeField] Transform bodyRotator;

    private bool _hasMouseMoved = false;
    private Vector3 _startRotation;
    private float _targetXRotation;
    private float _targetYRotation;
    private KeyboardZoneManager _zoneManager;

    #endregion

    #region Unity Methods
    public void Init()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _startRotation = transform.localEulerAngles;
        _zoneManager = KeyboardZoneManager.Instance;
    }
    public void Step()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) _hasMouseMoved = true;
        if (!_hasMouseMoved) return; 
        Vector2 mousePos = Input.mousePosition; 
        float screenWidth = Screen.width; 
        float screenHeight = Screen.height;
        
        float normalizedX = (mousePos.x / screenWidth - 0.5f) * 2f;
        float normalizedY = (mousePos.y / screenHeight - 0.5f) * 2f;

        var xOffset = xLookoffset;
        var yOffset = yLookoffset;


        if (_zoneManager.IsCollider)
        { 
            SensitivityOfCamera(_insideMaxRotationX, _insideMaxRotationY,normalizedX,normalizedY);
        }
        else
        {
            xOffset = 0;
            yOffset = 0;

            SensitivityOfCamera(_outsideMaxRotationX, _outsideMaxRotationY,normalizedX,normalizedY);
        }

        Quaternion targetRot = Quaternion.Euler(_targetXRotation, _targetYRotation, _startRotation.z);

        if (Mathf.Abs(_targetYRotation) >= rotationThreshold && !_zoneManager.IsCollider)
        {
            bodyRotator.Rotate(new Vector3(0, rotationSpeed * Mathf.Sign(_targetYRotation) * Time.deltaTime, 0));
        }
        else
            transform.localRotation = targetRot * Quaternion.Euler(xOffset, yOffset, 1);
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