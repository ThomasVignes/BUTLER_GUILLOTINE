using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterStocker : MonoBehaviour
{
    [SerializeField] int characterNumber;
    [SerializeField] List<Character> characters = new List<Character>();
    [SerializeField] TextMeshProUGUI text;

    bool cease;

    private void Start()
    {
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cease)
            return;

        Character c = other.gameObject.GetComponent<Character>();

        CameraDetector cam = other.gameObject.GetComponent<CameraDetector>();

        if (c != null)
        {
            characters.Add(c);
            UpdateText();
        }

        if (cam != null && !characters.Contains(GameManager.Instance.Player))
        {
            characters.Add(GameManager.Instance.Player);
            UpdateText();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cease)
            return;

        Character c = other.gameObject.GetComponent<Character>();

        CameraDetector cam = other.gameObject.GetComponent<CameraDetector>();

        if (c != null)
        {
            characters.Remove(c);
            UpdateText();
        }

        if (cam != null && characters.Contains(GameManager.Instance.Player))
        {
            characters.Remove(GameManager.Instance.Player);
            UpdateText();
        }
    }

    void UpdateText()
    {
        text.text = "Put " + (characterNumber - characters.Count) + " more people on here";

        if (characters.Count >= characterNumber)
        {
            cease = true;
            StartCoroutine(C_Load());
        }
    }

    IEnumerator C_Load()
    {
        GameManager.Instance.ScreenEffects.FadeTo(1, 2f);

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
