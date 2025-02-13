using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterSelector : MonoBehaviour
{
    [SerializeField] MenuChapter[] menuChapters;
    [SerializeField] Transform grid;
    [SerializeField] GameObject instance;

    public void Init()
    {
        foreach (var item in menuChapters)
        {
            GameObject go = Instantiate(instance);
            go.transform.SetParent(grid);
            go.transform.localScale = Vector3.one;

            MenuChapterButton button = go.GetComponent<MenuChapterButton>();

            button.Init(item.Title, item.Image, item.SceneReference);
        }
    }
}

[System.Serializable]
public class MenuChapter
{
    public string Title;
    public Sprite Image;
    public int SceneReference;
}