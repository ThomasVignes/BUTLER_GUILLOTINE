using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TypewriterPlayer : PlayerController
{
    public static TypewriterPlayer Instance; 
    #region Variables
    [Header("Settings")]
    [Tooltip("La vitesse du déplacement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _facingRotationSpeed = 5f;
    [SerializeField] LayerMask interactables, buttons;



    [Header("References")]
    [SerializeField] private TypewriterCameraController cameraController;
    [SerializeField] Transform ExitSpot;

    private CharacterController _characterController;
    private KeyboardZoneManager _currentZoneManager;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private bool _isMoving = false;
    private bool _goingToKeyboard = false;
    private bool _faceKeyboard = false;
    private bool _isGoingToExit = false;
    private bool _hasReachedExit = false;
    #endregion

    #region Unity Methods

    public override void Init()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        base.Init();

        _characterController = GetComponent<CharacterController>();

        _targetPosition = transform.position;

        cameraController.Init();

        agent.enabled = false;
    }

    public override void ConstantStep()
    {
        cameraController.Step();
    }

    public override void Step()
    {
        //base.Step();

        if (_currentZoneManager != null && _currentZoneManager.IsCollider)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10, buttons))
                {
                    var letter = hit.transform.gameObject.GetComponent<ClickDetectionLetterKey>();

                    if (letter != null)
                        letter.ClickLetter();
                }
            }
        }

        LegacyMovement();
    }

    private void LegacyMovement()
    {
        if (_currentZoneManager != null && _currentZoneManager.IsCollider && !_isGoingToExit && !_hasReachedExit) return;

        /*
        if (Input.GetMouseButtonDown(0) && !_goingToKeyboard && !_isGoingToExit)
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
            {
                _targetPosition = ray.GetPoint(distance);
                _isMoving = true;
            }
        }
        */
        

        if (_isMoving)
        {
            Vector3 direction = _targetPosition - transform.position;
            direction.y = 0f;

            if (_faceKeyboard)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _facingRotationSpeed);

                if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.001f)
                {
                    _currentZoneManager.IsCollider = true;
                    _isMoving = false;
                    _faceKeyboard = false;

                    _currentZoneManager.OnReach?.Invoke();
                }

                return;
            }

            if (direction.magnitude < 0.05f)
            {
                _characterController.Move(Vector3.zero);

                if (_isGoingToExit && !_hasReachedExit)
                {
                    _hasReachedExit = true;
                    _targetRotation = ExitSpot.rotation;
                    return;
                }

                if (_hasReachedExit)
                {
                    float angle = Quaternion.Angle(transform.rotation, _targetRotation);
                    if (angle < 0.001f)
                    {
                        //transform.rotation = _targetRotation;
                        _currentZoneManager.IsCollider = false;
                        _isGoingToExit = false;
                        _hasReachedExit = false;
                        _isMoving = false;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _facingRotationSpeed);
                        _isMoving = true;
                    }
                    return;
                }

                if (_goingToKeyboard)
                {
                    _goingToKeyboard = false;
                    _faceKeyboard = true;
                    cameraController.LockMode(true, _currentZoneManager.CameraOffset, _currentZoneManager.CameraConstraints);

                    _targetRotation = Quaternion.Euler(_currentZoneManager.EnterRotation);
                }
                return;
            }
            _characterController.Move(direction.normalized * _speed * Time.deltaTime);
            if (_goingToKeyboard || _isGoingToExit || _hasReachedExit)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
            }
        }
    }
    #endregion

    #region Public Methods
    public void GoToKeyboard(KeyboardZoneManager zone)
    {
        if (_goingToKeyboard) return;

        _currentZoneManager = zone;

        _targetPosition = _currentZoneManager.EnterPosition;

        var lookDir = Vector3.Normalize(_currentZoneManager.EnterPosition - transform.position);

        lookDir.y = 0;

        _targetRotation = Quaternion.LookRotation(lookDir);
        _isMoving = true;
        _goingToKeyboard = true;
    }
    public void GoToExit()
    {
        cameraController.LockMode(false, Vector2.zero, Vector2.zero);

        _targetPosition = ExitSpot.position;
        Vector3 direction = (ExitSpot.position - transform.position).normalized;
        direction.y = 0;
        _targetRotation = Quaternion.LookRotation(direction);
        _isMoving = true;
        _isGoingToExit = true;
        _hasReachedExit = false;
    }
    #endregion
}