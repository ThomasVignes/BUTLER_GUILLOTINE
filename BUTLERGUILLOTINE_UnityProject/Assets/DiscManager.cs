using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] List<DiscSong> songs = new List<DiscSong>();

    [Header("References")]
    [SerializeField] Animator ruthAnimator;
    [SerializeField] Animator discPlayerAnimator, cameraAnimator;
    [SerializeField] TextMeshProUGUI title;

    public int currentIndex;
    bool hasDisc, playing;

    private void Start()
    {
        title.text = "";
        currentIndex = -1;
    }

    public void Play()
    {
        if (currentIndex == -1) return;

        if (playing) return;

        playing = true;

        StartCoroutine(C_PlaySong());
    }

    IEnumerator C_PlaySong()
    {
        ruthAnimator.SetTrigger("Play");

        yield return new WaitForSeconds(0.4f);

        cameraAnimator.SetBool("Playing", true);

        yield return new WaitForSeconds(0.05f);

        discPlayerAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(2f);

        discPlayerAnimator.SetTrigger("Spin");
    }

    public void Stop()
    {
        if (!playing) return;

        playing = false;

        StartCoroutine(C_StopSong());
    }

    IEnumerator C_StopSong()
    {
        discPlayerAnimator.SetTrigger("Stop");

        yield return new WaitForSeconds(0.5f);

        cameraAnimator.SetBool("Playing", false);

        yield return new WaitForSeconds(0.45f);


        ruthAnimator.SetTrigger("Stop");
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