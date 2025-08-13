using System.Collections;
using System.Collections.Generic;
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

    private CharacterController _characterController;
    private KeyboardZoneManager _zoneManager;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private bool _isMoving = false;
    private bool _goingToKeyboard = false;
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
        _zoneManager = KeyboardZoneManager.Instance;
        _targetPosition = transform.position;
    }
    public override void Step()
    {
        //base.Step();

        
        if (_zoneManager.IsCollider && !_isGoingToExit && !_hasReachedExit) return;
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
        if (_isMoving)
        {
            Vector3 direction = _targetPosition - transform.position;
            direction.y = 0f;

            if (direction.magnitude < 0.05f)
            {
                _characterController.Move(Vector3.zero);

                if (_isGoingToExit && !_hasReachedExit)
                {
                    _hasReachedExit = true;
                    _targetRotation = Quaternion.Euler(_zoneManager.ExitRotation);
                    return;
                }

                if (_hasReachedExit)
                {
                    float angle = Quaternion.Angle(transform.rotation, _targetRotation);
                    if (angle < 1f)
                    {
                        transform.rotation = _targetRotation;
                        _zoneManager.IsCollider = false;
                        _isGoingToExit = false;
                        _hasReachedExit = false;
                        _isMoving = false;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _rotationSpeed);
                        _isMoving = true;
                    }
                    return;
                }

                if (_goingToKeyboard)
                {
                    _zoneManager.IsCollider = true;
                    transform.rotation = _targetRotation;
                    _goingToKeyboard = false;
                    _isMoving = false;
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
    public void GoToKeyboard()
    {
        if (_goingToKeyboard) return;
        _targetPosition = _zoneManager.EnterPosition;
        _targetRotation = Quaternion.Euler(_zoneManager.EnterRotation);
        _isMoving = true;
        _goingToKeyboard = true;
    }
    public void GoToExit()
    {
        _targetPosition = _zoneManager.ExitPosition;
        Vector3 direction = (_zoneManager.ExitPosition - transform.position).normalized;
        direction.y = 0;
        _targetRotation = Quaternion.LookRotation(direction);
        _isMoving = true;
        _isGoingToExit = true;
        _hasReachedExit = false;
    }
    #endregion
}