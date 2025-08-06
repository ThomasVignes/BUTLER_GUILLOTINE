using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PaperMover : MonoBehaviour
{
    #region Variables
    [Header("Deplacement horizontal")]
    [Tooltip("Distance entre chaque position")]
    [SerializeField] private float _distanceLeft = 0.1f;
    
    [Header("Deplacement vertical")]
    [Tooltip("Distance entre chaque ligne")]
    [SerializeField] private float _distanceUp = 5f;

    [Tooltip("Vitesse de deplacement")]
    [SerializeField] private float _moveSpeed = 300f;
    
    private Vector3 _initialPosition;
    private Vector3 _targetPosition;
    private bool _isMoving = false;
    private int _lineIndex = 0;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _initialPosition = transform.position;
        _targetPosition = transform.position;
    }
    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
            }
        }
    }
    #endregion
    
    #region Public Methods
    public void MoveLeft()
    {
        if (_isMoving) return;
        _targetPosition += Vector3.left * _distanceLeft;
        _isMoving = true;
    }
    public void MoveUp()
    {
        if (_isMoving) return;
        float newY = _initialPosition.y + (_distanceUp * _lineIndex);
        _targetPosition = new Vector3(_initialPosition.x, newY, transform.position.z);
        _lineIndex++;
        _isMoving = true;
    }
    public void ResetPaperPosition()
    {
        _targetPosition = _initialPosition;
        _lineIndex = 0;
        _isMoving = true;
    }
    #endregion
}