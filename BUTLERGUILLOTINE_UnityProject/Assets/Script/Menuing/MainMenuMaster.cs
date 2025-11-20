using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuMaster : MonoBehaviour
{
    public static MainMenuMaster Instance;

    [Header("Values")]
    public bool CanInput;
    public bool AutoQuit;
    [SerializeField] float travelSpeed;
    [SerializeField] float fadeOutSpeed;

    [Header("Modularity")]
    [SerializeField] float silenceTime;
    [SerializeField] StudioEventEmitter emitter;

    [Header("References")]
    [SerializeField] SettingsManager settingsManager;
    [SerializeField] Texture2D BaseCursor;
    [SerializeField] GameObject chapterSelect;
    [SerializeField] private Image BlackScreen;
    [SerializeField] Transform Elevator;
    [SerializeField] Transform[] Spots;
    [SerializeField] StudioEventEmitter arriveSound;
    [SerializeField] GameObject eventSystem;
    [SerializeField] ChapterSelector chapterSelector;
    [SerializeField] Animator startMenuAnimator;
    [SerializeField] Button continueButton;

    bool startMenuOpen;

    [Header("Credits")]
    [SerializeField] CreditsManager creditsManager;


    bool disclaimerMode, moving;
    int targetSpot;

    bool fadeOut;
    float themeVolume;


    private void Awake()
    {
        Instance = this;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        Cursor.SetCursor(BaseCursor, new Vector2(18, 13), CursorMode.Auto);

        creditsManager.Init(this);
    }

    private void Start()
    {
        settingsManager.Init();

        StartCoroutine(C_ShowMenu());

        /*
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(FirstMenuButton);
        */

        Screen.fullScreen = true;

        Elevator.position = Spots[targetSpot].position;
        
        chapterSelector.Init();

        bool canContinue = PersistentData.Instance.BuildNavigator.CanContinue();

        if (!canContinue)
        {
            continueButton.interactable = false;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }


        emitter.EventInstance.getVolume(out themeVolume);
    }

    private void Update()
    {
        if (fadeOut)
        {
            if (themeVolume > 0)
            {
                themeVolume -= Time.deltaTime * fadeOutSpeed;

                if (themeVolume > 0)
                    emitter.EventInstance.setVolume(themeVolume);
                else
                    emitter.Stop();
            }
        }

        if (!CanInput)
            return;

        if (!moving)
            return;

        var spot = Spots[targetSpot].position;

        if (Vector3.Distance(Elevator.position, spot) > Time.deltaTime)
            Elevator.position = Vector3.MoveTowards(Elevator.position, spot, travelSpeed * Time.deltaTime);
        else
        {
            arriveSound.Play();
            moving = false;
        }
    }

    public void OpenSite()
    {
        Application.OpenURL("https://store.steampowered.com/app/3617650/BUTLER_GUILLOTINE/");
    }

    public void RollCredits()
    {
        CanInput = false;

        BlackFadeTo(1, 1);

        creditsManager.Roll();
    }

    public void EndCredits()
    {
        creditsManager.ResetCredits();

        BlackFadeTo(0, 1);

        CanInput = true;
    }

    public void QuitGame()
    {
        PersistentData.Instance.BuildNavigator.RequestQuit();
    }

    public void ToggleStartMenu()
    {
        startMenuOpen = !startMenuOpen;

        startMenuAnimator.SetBool("Open", startMenuOpen);
    }

    public void HideStartMenu()
    {
        startMenuOpen = false;

        startMenuAnimator.SetBool("Open", startMenuOpen);
    }

    public void ChangeSpots(int index)
    {
        if (!CanInput)
            return;

        targetSpot = index;
        moving = true;

        if (index > 0)
            HideStartMenu();
    }

    public void StartGame(bool loadSave)
    {
        if (!CanInput)
            return;

        CanInput = false;
        eventSystem.SetActive(false);

        StartCoroutine(C_StartGame(loadSave, false));
    }

    public void GoToDiscs()
    {
        if (!CanInput)
            return;

        CanInput = false;
        eventSystem.SetActive(false);

        StartCoroutine(C_StartGame(false, true));
    }

    IEnumerator C_StartGame(bool loadSave, bool toDiscs)
    {
        BlackFadeTo(1, 4f);

        fadeOut = true;

        //Test
        PersistentData.Instance.SteamAchievementManager.TriggerAchievement("BLANK", true);

        if (!toDiscs)
            yield return new WaitForSeconds(9f);
        else
            yield return new WaitForSeconds(7f);

        if (!toDiscs)
            yield return new WaitForSeconds(silenceTime);

        if (toDiscs)
            SceneManager.LoadScene("Discs");
        else
        {
            if (loadSave)
                PersistentData.Instance.BuildNavigator.Continue();
            else
                PersistentData.Instance.BuildNavigator.NextScene();
        }
    }

    IEnumerator C_ShowMenu()
    {
        BlackFadeTo(1, 0.0001f);
        eventSystem.SetActive(false);

        yield return new WaitForSeconds(1f);

        BlackFadeTo(0, 3f);

        yield return new WaitForSeconds(1.8f);

        CanInput = true;
        eventSystem.SetActive(true);
    }

    public void ToggleChapterSelect(bool toggle)
    {
        chapterSelect.SetActive(toggle);
    }

    public void BlackFadeTo(int value)
    {
        BlackScreen.DOKill();
        BlackScreen.DOFade(value, 1.3f);
    }

    public void BlackFadeTo(int value, float speed)
    {
        BlackScreen.DOKill();
        BlackScreen.DOFade(value, speed);
    }
}
