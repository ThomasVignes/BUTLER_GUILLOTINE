using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LetterManager : MonoBehaviour
{
    #region Variables
    public static LetterManager Instance;
    
    [Header("References")] 
    [Tooltip("Reference du script PaperMover")]
    [SerializeField] private PaperMover _paperMover;

    [Tooltip("Reference vers les textes TMP a afficher")]
    [SerializeField] private List<TextMeshPro> _texts = new List<TextMeshPro>();

    [Header("Settings")]
    [Tooltip("Maximum de lettres sur une ligne")] 
    [SerializeField] private int _letterMaxCount = 21;

    private bool _isShiftPressed = false;
    private bool _isLockShiftPressed = false;
    private int _countLines = 0;
    private string _word = "";
    private List<string> _wordList = new();
    private bool _isLocked = false;
    private bool _isLineFull = false;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < _texts.Count; i++) _texts[i].text = "";
    }

    #endregion

    #region Public Methods

    public void AddLetter(char letter)
    {
        if (_isLocked || _isLineFull)
        {
            Debug.Log("locke");
            return;
        }

        if (_word.Length >= _letterMaxCount)
        {
            if (_countLines >= _texts.Count - 1)
            {
                Debug.Log("derniere ligne !!!!");
                _isLocked = true;
                _isLineFull = true;
                return;
            }

            Debug.Log("Depasse !");
            
            // AddNewLine(); //si aller a la ligne auto
            return;
        }

        _paperMover.MoveLeft();
        _word += letter;
        UpdateText();

        if (_isShiftPressed) _isShiftPressed = false;
    }

    public void AddNewLine()
    {
        if (_isLocked) return;

        if (!string.IsNullOrWhiteSpace(_word))
        {
            _wordList.Add(_word);
            _texts[_countLines].text = _word;
        }

        if (_countLines >= _texts.Count - 1)
        {
            _isLocked = true;
            TextChecker.Instance.CheckText(_wordList);
            return;
        }

        _countLines++;
        _word = "";
        _isLineFull = false;
        _paperMover.MoveUp();
        UpdateText();
    }

    public bool IsShiftActive()
    {
        return _isShiftPressed || _isLockShiftPressed;
    }

    public void EnableShift()
    {
        _isShiftPressed = !_isShiftPressed;
    }

    public void EnableLockShift()
    {
        _isLockShiftPressed = !_isLockShiftPressed;
    }
    public bool IsLineFull()
    {
        return _word.Length >= _letterMaxCount;
    }

    public bool IsCurrentLineEmpty()
    {
        return string.IsNullOrWhiteSpace(_word);
    }

    public void ResetAll()
    {
        Debug.Log("Reinisialise");
        _countLines = 0;
        _word = "";
        _wordList.Clear();
        _isLocked = false;
        _isLineFull = false;
        _isShiftPressed = false;
        _isLockShiftPressed = false;

        for (int i = 0; i < _texts.Count; i++) _texts[i].text = "";
        _paperMover.ResetPaperPosition();
        UpdateText();
    }

    #endregion

    #region Private Methods

    private void UpdateText()
    {
        if (_texts[_countLines] != null) _texts[_countLines].text = _word;
    }

    private void ClearWord()
    {
        if (_countLines >= _texts.Count)
        {
            Debug.Log("no spaaaace");
            _countLines = _texts.Count - 1;
            _isLocked = true;
            return;
        }

        _word = "";
        UpdateText();
    }
    #endregion
}