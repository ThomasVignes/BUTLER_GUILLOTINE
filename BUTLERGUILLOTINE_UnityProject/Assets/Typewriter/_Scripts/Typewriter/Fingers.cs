using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static ClickDetectionLetterKey;

public class Fingers : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private Finger _finger;
    [SerializeField] private Transform _targetGameObject;
    [SerializeField] private Transform _fingerTip;
    
    [Header("Animation")]
    [Tooltip("rotation vers le bas")]
    [SerializeField] private Vector3 _downRotation = new Vector3(1.62f, -55.921f, 1.411f);
    [Tooltip("Vitesse quand ca descend")]
    [SerializeField] private float _speedRotation = 2f;
    [SerializeField] private float _waitRotation = 0.1f;
    [SerializeField] private float _speedPosition = 10f;
    [SerializeField] private float _positionUpFinger = 0.015f; 
    
    private Quaternion _initialRotation, _targetRotation;
    private Coroutine _coroutineRotation, _coroutinePosition;
    private Vector3 _initialPosition;
    #endregion
    
    #region Unity Methods
    private void Start()
    {
        _initialRotation = transform.localRotation;
        _targetRotation = Quaternion.Euler(_downRotation);
        _initialPosition = _targetGameObject.localPosition;
    }
    #endregion
    
    #region Public Methods
    public void Press(Vector3 targetPosition, ClickDetectionLetterKey key)
    {
        if (_coroutineRotation != null)
            StopCoroutine(_coroutineRotation);

        if (_coroutinePosition != null)
            StopCoroutine(_coroutinePosition);

        _coroutinePosition = StartCoroutine(AnimateFingerPressPosition(targetPosition, key));
        _coroutineRotation = StartCoroutine(AnimateFingerPressRotation());
    }
    public void PressWithoutKeyAnimation(Vector3 targetPosition, ClickDetectionLetterKey key)
    {
        if (_coroutineRotation != null)
            StopCoroutine(_coroutineRotation);
        if (_coroutinePosition != null)
            StopCoroutine(_coroutinePosition);

        _coroutinePosition = StartCoroutine(AnimateFingerWithoutKey(targetPosition,key));
        _coroutineRotation = StartCoroutine(AnimateFingerPressRotation());
    }

    public Finger GetFinger() => _finger;
    #endregion
    
    #region Animation
    private IEnumerator AnimateFingerPressRotation()
    {
        while (Quaternion.Angle(transform.localRotation, _targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, _speedRotation * 100 * Time.deltaTime);
            yield return null;
        }
        
        yield return new WaitForSeconds(_waitRotation);
        
        while (Quaternion.Angle(transform.localRotation, _initialRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _initialRotation, _speedRotation * 100 * Time.deltaTime);
            yield return null;
        }
        transform.localRotation = _initialRotation;
    }
    private IEnumerator AnimateFingerPressPosition(Vector3 targetPosition, ClickDetectionLetterKey key)
    {
        targetPosition += Vector3.up * _positionUpFinger;
        if (key.IsSpaceKey) targetPosition += Vector3.back * key.DownHandSpaceKey;

        Vector3 offset = targetPosition - _fingerTip.position;
        Vector3 armTargetPosition = _targetGameObject.localPosition  + offset;

        while (Vector3.Distance(_targetGameObject.localPosition , armTargetPosition) > 0.001f)
        {
            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , armTargetPosition, _speedPosition * Time.deltaTime);
            yield return null;
        }

        Coroutine pressCoroutine = key.StartCoroutine(key.AnimatePress());

        while (key.IsAnimating)
        {
            Vector3 targetKeyPos = key.GetCurrentKeyPosition() + Vector3.up * _positionUpFinger;
            Vector3 offset2 = targetKeyPos - _fingerTip.position;
            Vector3 followTarget = _targetGameObject.localPosition  + offset2;

            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , followTarget, _speedPosition * Time.deltaTime);
            yield return null;
        }

        while (Vector3.Distance(_targetGameObject.localPosition , _initialPosition) > 0.001f)
        {
            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , _initialPosition, _speedPosition * Time.deltaTime);
            yield return null;
        }
        _targetGameObject.localPosition  = _initialPosition;
    }
    private IEnumerator FollowKeyWhilePressed(ClickDetectionLetterKey key)
    {
        while (key.IsAnimating) 
        {
            Vector3 target = key.GetCurrentKeyPosition() + Vector3.up * _positionUpFinger;
            Vector3 offset = target - _fingerTip.position;
            Vector3 armTarget = _targetGameObject.localPosition  + offset;
            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition, armTarget, _speedPosition * Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator AnimateFingerWithoutKey(Vector3 targetPosition, ClickDetectionLetterKey key)
    {
        targetPosition += Vector3.up * _positionUpFinger;
        if (key.IsSpaceKey) targetPosition += Vector3.back * key.DownHandSpaceKey;

        Vector3 offset = targetPosition - _fingerTip.position;
        Vector3 armTargetPosition = _targetGameObject.localPosition  + offset;

        while (Vector3.Distance(_targetGameObject.localPosition , armTargetPosition) > 0.001f)
        {
            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , armTargetPosition, _speedPosition * Time.deltaTime);
            yield return null;
        }

        Coroutine pressCoroutine = key.StartCoroutine(key.AnimateSoftPress());

        while (key.IsAnimating)
        {
            Vector3 targetKeyPos = key.GetCurrentKeyPosition() + Vector3.up * _positionUpFinger;
            Vector3 offset2 = targetKeyPos - _fingerTip.position;
            Vector3 followTarget = _targetGameObject.localPosition  + offset2;

            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , followTarget, _speedPosition * Time.deltaTime);
            yield return null;
        }

        while (Vector3.Distance(_targetGameObject.localPosition , _initialPosition) > 0.001f)
        {
            _targetGameObject.localPosition  = Vector3.MoveTowards(_targetGameObject.localPosition , _initialPosition, _speedPosition * Time.deltaTime);
            yield return null;
        }
        _targetGameObject.localPosition  = _initialPosition;
    }
    #endregion
}
