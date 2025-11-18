using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] List<DiscSong> songs = new List<DiscSong>();
    [SerializeField] StudioEventEmitter crackle;

    [Header("Music playing")]
    [SerializeField] float fadeOutSpeed;

    [Header("References")]
    [SerializeField] Animator ruthAnimator;
    [SerializeField] Animator discPlayerAnimator, cameraAnimator;

    [Header("Text references")]
    [SerializeField] TextMeshProUGUI jerTitle;
    [SerializeField] TextMeshProUGUI tableTitle;

    public int currentIndex;
    bool hasDisc, playing;
    bool edited, hasEdited;

    bool fadeOut;
    float themeVolume;

    EventInstance currentInstance;
    Coroutine playSong;

    private void Start()
    {
        currentIndex = -1;
        UpdateTitles();

        discPlayerAnimator.SetFloat("SpinSpeed", 1);
    }

    private void Update()
    {
        if (fadeOut)
        {
            if (themeVolume > 0)
            {
                themeVolume -= Time.deltaTime * fadeOutSpeed;

                if (themeVolume > 0)
                    crackle.EventInstance.setVolume(themeVolume/2);
                else
                    crackle.Stop();
            }
        }
    }

    public void PlaySong(bool edited)
    {
        StopSong();

        EventReference reference = songs[currentIndex].original;

        if (edited && !songs[currentIndex].edited.IsNull)
            reference = songs[currentIndex].edited;

        playSong = StartCoroutine(C_PlaySong(reference));
    }

    IEnumerator C_PlaySong(EventReference reference)
    {
        fadeOut = false;
        themeVolume = 1;
        crackle.EventInstance.setVolume(themeVolume / 2);

        crackle.Stop();
        crackle.Play();

        yield return new WaitForSeconds(2.5f);

        fadeOut = true;

        currentInstance = RuntimeManager.CreateInstance(reference);

        currentInstance.start();
        currentInstance.release();
    }

    public void StopSong()
    {
        if (playSong != null) StopCoroutine(playSong);

        crackle.Stop();
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void Play()
    {
        if (currentIndex == -1) return;

        if (playing) return;

        playing = true;

        StartCoroutine(C_Play());
    }

    IEnumerator C_Play()
    {
        ruthAnimator.SetTrigger("Play");

        yield return new WaitForSeconds(0.4f);

        cameraAnimator.SetBool("Playing", true);

        yield return new WaitForSeconds(0.05f);

        discPlayerAnimator.SetTrigger("Start");
        discPlayerAnimator.SetFloat("SpinSpeed", 1);

        yield return new WaitForSeconds(1.4f);

        discPlayerAnimator.SetTrigger("Spin");

        PlaySong(false);
    }

    public void Stop()
    {
        if (!playing) return;

        playing = false;

        StartCoroutine(C_StopSong());
    }

    IEnumerator C_StopSong()
    {
        StopSong();
        discPlayerAnimator.SetTrigger("Stop");

        yield return new WaitForSeconds(0.5f);

        cameraAnimator.SetBool("Playing", false);

        yield return new WaitForSeconds(0.45f);

        ruthAnimator.SetTrigger("Stop");

        discPlayerAnimator.SetBool("Edited", false);
        discPlayerAnimator.SetTrigger("ResetButton");
        edited = false;
    }

    public void NextDisc()
    {
        UpdateDisc(true);
    }

    public void PreviousDisc()
    {
        UpdateDisc(false);
    }

    public void ToggleEdited()
    {
        if (hasEdited)
        {
            edited = !edited;

            PlaySong(edited);

            float spinValue = 1;

            if (edited)
                spinValue = -1;

            discPlayerAnimator.SetFloat("SpinSpeed", spinValue);
            discPlayerAnimator.SetBool("Edited", edited);
        }
        else
            discPlayerAnimator.SetTrigger("NoEdited");
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

        if (currentIndex >= 0 && currentIndex < songs.Count)
            hasEdited = !songs[currentIndex].edited.IsNull;


        //Animation logic
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

        SwitchDisc();
    }

    void ToggleDisc(bool active)
    {
        hasDisc = active;

        if (hasDisc)
        {
            ruthAnimator.SetTrigger("GrabDisc");
            UpdateTitles();
        }
        else
        {
            ruthAnimator.SetTrigger("HideDisc");
            UpdateTitles();
        }
    }

    void UpdateTitles()
    {
        string content = "Select a song";
        string jerAdditional = "";
        string tableAdditionnal = "";
        string description = "description";

        if (currentIndex > -1)
        {
            content = songs[currentIndex].Name;

            string composer = songs[currentIndex].Composer;
            string performer = songs[currentIndex].Performer;

            jerAdditional = "\n\n" + composer + "\n" + performer;
            if (composer == performer) jerAdditional = "\n\n" + composer;

            tableAdditionnal = "\n\nComposition: " + composer + "\nPerformance: " + performer;

            description = "\n\n" + songs[currentIndex].Description;
        }



        jerTitle.text = content + jerAdditional;
        tableTitle.text = content + tableAdditionnal + description;
    }

    void SwitchDisc()
    {
        ruthAnimator.SetTrigger("SwitchDisc");
        UpdateTitles();
    }
}

[System.Serializable]
public class DiscSong
{
    public string Name;
    public string Composer;
    public string Performer;
    public string Description;
    public EventReference original;
    public EventReference edited;
}