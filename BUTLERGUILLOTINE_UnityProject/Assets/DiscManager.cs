using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    [SerializeField] List<DiscSong> songs = new List<DiscSong>();
    [SerializeField] Animator ruthAnimator;
    [SerializeField] TextMeshProUGUI title;

    public int currentIndex;
    bool hasDisc;

    private void Start()
    {
        title.text = "";
        currentIndex = -1;
    }

    public void NextDisc()
    {
        UpdateDisc(true);
    }

    public void PreviousDisc()
    {
        UpdateDisc(false);
    }

    void UpdateDisc(bool increment)
    {
        bool hadDisc = hasDisc;


        if (currentIndex == -1)
        {
            if (!increment)
                currentIndex = songs.Count;
        }


        if (increment)
            currentIndex++;
        else
            currentIndex--;


        if (currentIndex < 0 || currentIndex >= songs.Count)
        {
            currentIndex = -1;
            ToggleDisc(false);
            return;
        }

        if (!hadDisc)
        {
            ToggleDisc(true);
            return;
        }

        IncrementDisc();
    }

    void ToggleDisc(bool active)
    {
        hasDisc = active;

        if (hasDisc)
        {
            ruthAnimator.SetTrigger("GrabDisc");
            title.text = currentIndex.ToString();
        }
        else
        {
            ruthAnimator.SetTrigger("HideDisc");
            title.text = "";
        }
    }

    void IncrementDisc()
    {
        title.text = currentIndex.ToString();
        ruthAnimator.SetTrigger("SwitchDisc");
    }
}

[System.Serializable]
public class DiscSong
{
    public string Name;
}