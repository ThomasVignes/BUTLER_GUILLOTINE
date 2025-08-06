using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextChecker : MonoBehaviour
{
    #region Variables
    public static TextChecker Instance;

    [Header("Text a verifier")]
    [SerializeField] private List<string> _textToCheck = new();
     #endregion
    
    #region Unity Methods
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        for (int i = 0; i < _textToCheck.Count; i++)
        {
            _textToCheck[i] = _textToCheck[i].ToUpper();
            Debug.Log(_textToCheck[i]);
        }
    }
    #endregion
    
    #region Public Methods
    public void CheckText(List<string> text)
    {
        if (IsCorrect(text))Debug.Log("felicitation tu slay fort !");
        else
        {
            Debug.Log("ah bah cest faux ma cocotte!");
            LetterManager.Instance.ResetAll(); 
        }
    }
    public bool IsCorrect(List<string> input)
    {
        if (input.Count != _textToCheck.Count) return false;
        for (int i = 0; i < _textToCheck.Count; i++) if (_textToCheck[i] != input[i]) return false;
        return true;
    }
    #endregion
}
