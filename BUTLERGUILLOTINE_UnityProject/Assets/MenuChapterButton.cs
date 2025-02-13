using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuChapterButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    int sceneReference;

    public void Init(string txt, Sprite img, int sceneRef)
    {
        image.sprite = img;
        text.text = txt;
        sceneReference = sceneRef;
    }

    public void LoadChapter()
    {
        SceneManager.LoadScene(sceneReference);
    }
}
