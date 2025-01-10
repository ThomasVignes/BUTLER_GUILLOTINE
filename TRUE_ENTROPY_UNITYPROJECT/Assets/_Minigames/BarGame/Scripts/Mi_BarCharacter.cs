using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mi_BarCharacter : MonoBehaviour
{
    public string CurrentDialogue;
    [SerializeField] private List<string> dialogues = new List<string>();

    [SerializeField] private TextMeshProUGUI TMP;
    [SerializeField] private HorizontalLayoutGroup layout;

    private int yes = 0;

    private void Start()
    {
        CurrentDialogue = dialogues[0];
        UpdateLayout();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            yes++;
            TMP.text = dialogues[yes];
            UpdateLayout();
        }


    }

    private void UpdateLayout()
    {
        layout.gameObject.SetActive(dialogues[yes] != "");
        Canvas.ForceUpdateCanvases();
        layout.enabled = false;
        layout.enabled = true;
    }
}
