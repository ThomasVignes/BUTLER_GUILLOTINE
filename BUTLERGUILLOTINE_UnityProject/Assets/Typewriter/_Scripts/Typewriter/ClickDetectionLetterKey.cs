using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ClickDetectionLetterKey : MonoBehaviour
{
    public enum Finger
    {
        LeftIndex,
        RightIndex,
    }

    #region Variables
    [Header("Character")]
    [Tooltip("Lettre a afficher dans le texte et la console")]
    [SerializeField] private KeyChar _keyChar;
    public bool IsSpaceKey = false;
    [SerializeField] private bool _isEnterKey;
    [SerializeField] private bool _isShift;
    [SerializeField] private bool _isLockeShift;
    [SerializeField] private bool _isNotLetter;
    [SerializeField] private bool _isExitKey;
    
    [Tooltip("quel doigt sur la touche")]
    [SerializeField] private Finger _assignedFinger;
    
    [Tooltip("Champ TextMeshProUGUI qui affichera la lettre")]
    [SerializeField] private TextMeshPro  _text;
    [SerializeField] private TextMeshPro _shiftedText;

    [Header("If SpaceKey")]
    [Tooltip("Pour que le doigt sois sur la touche espace et non au milieu")]
    public float DownHandSpaceKey;
    
    private Fingers[] _allFingers;
    private Vector3 _initialPosition;
    private Vector3 _downPosition;
    private bool _isAnimating = false;
    #endregion
    
    #region Unity Methods
    private void Start()
    {
        _allFingers = FindObjectsOfType<Fingers>();
        _text.text = _keyChar.NormalChar.ToString();
        _text.text = _text.text.ToUpper();
        if(_shiftedText != null)
        {
            _shiftedText.text = _keyChar.ShiftChar.ToString();
            _shiftedText.text = _shiftedText.text.ToUpper();
        }
        _initialPosition = transform.position;
        _downPosition = _initialPosition + Vector3.down * AnimationSettingsManager.Instance.DownDistance;
    }
    public void ClickLetter()
    {
        if (_isAnimating) return;
        foreach (var finger in _allFingers)
        {
            if (finger.GetFinger() == _assignedFinger)
            {
                finger.Press(transform.position, this);
                break;
            }
        }
        if (_isExitKey)
        {
            Debug.Log("touche exit");
            TypewriterPlayer.Instance.GoToExit(); 
            return;
        }

        if (_isShift)
        {
            LetterManager.Instance.EnableShift();
            return;
        }        
        if (_isLockeShift)
        {
            LetterManager.Instance.EnableLockShift();
            return;
        }        
        
        if (_isEnterKey)
        {
            if (LetterManager.Instance.IsCurrentLineEmpty())
            {
                foreach (var finger in _allFingers)
                {
                    if (finger.GetFinger() == _assignedFinger)
                    {
                        finger.PressWithoutKeyAnimation(transform.position, this);
                        break;
                    }
                }
                return;
            }
            LetterManager.Instance.AddNewLine();
            return;
        }
        if (_isNotLetter) return;

        if (LetterManager.Instance.IsLineFull())
        {
            foreach (var finger in _allFingers)
            {
                if (finger.GetFinger() == _assignedFinger)
                {
                    finger.PressWithoutKeyAnimation(transform.position,this); 
                    break;
                }
            }
            return;
        }
        foreach (var finger in _allFingers)
        {
            if (finger.GetFinger() == _assignedFinger)
            {
                finger.Press(transform.position, this);
                break;
            }
        }

        LetterManager.Instance.AddLetter(LetterManager.Instance.IsShiftActive() ? _keyChar.ShiftChar : _keyChar.NormalChar);
    }
    #endregion
    
    #region Public Methods
    public Vector3 GetCurrentKeyPosition()
    {
        return transform.position;
    }
    public bool IsAnimating => _isAnimating;
    #endregion
    
    #region Animation
    public IEnumerator AnimatePress()
    {
        _isAnimating = true;

        while (Vector3.Distance(transform.position, _downPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _downPosition, AnimationSettingsManager.Instance.Speed * Time.deltaTime);
            yield return null;
        }
        
        EffectsManager.Instance.audioManager.Play("Click");
        
        while (Vector3.Distance(transform.position, _initialPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _initialPosition, AnimationSettingsManager.Instance.Speed * Time.deltaTime);
            yield return null;
        }
        transform.position = _initialPosition;
        _isAnimating = false;
    }
    public IEnumerator AnimateSoftPress()
    {
        _isAnimating = true;

        float ratio = AnimationSettingsManager.Instance.SoftPress;
        Vector3 softDown = _initialPosition + Vector3.down * (AnimationSettingsManager.Instance.DownDistance * ratio);

        while (Vector3.Distance(transform.position, softDown) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, softDown, AnimationSettingsManager.Instance.Speed * Time.deltaTime);
            yield return null;
        }
        
        EffectsManager.Instance.audioManager.Play("Click");

        while (Vector3.Distance(transform.position, _initialPosition) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _initialPosition, AnimationSettingsManager.Instance.Speed * Time.deltaTime);
            yield return null;
        }
        transform.position = _initialPosition;
        _isAnimating = false;
    }
    #endregion
}