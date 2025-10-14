using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Modularity")]
    [SerializeField] float silenceTime;
    [SerializeField] StudioEventEmitter emitter;

    [Header("References")]
    [SerializeField] Texture2D BaseCursor;
    [SerializeField] GameObject chapterSelect;
    [SerializeField] private Image BlackScreen;
    [SerializeField] Transform Elevator;
    [SerializeField] Transform[] Spots;
    [SerializeField] StudioEventEmitter arriveSound;
    [SerializeField] GameObject eventSystem;
    [SerializeField] ChapterSelector chapterSelector;

    [Header("MainMenuing")]
    [SerializeField] private GameObject FirstMenuButton;

    [Header("OptionsMenuing")]
    [SerializeField] private GameObject FirstOptionsButton;
    [SerializeField] private Slider masterVolume;

    [Header("Credits")]
    [SerializeField] CreditsManager creditsManager;


    bool disclaimerMode, moving;
    int targetSpot;

    private void Awake()
    {
        Instance = this;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        Cursor.SetCursor(BaseCursor, new Vector2(18, 13), CursorMode.Auto);

        creditsManager.Init(this);

        if (PersistentData.Instance != null)
            masterVolume.value = PersistentData.Instance.MasterVolume;
    }

    private void Start()
    {
        StartCoroutine(C_ShowMenu());

        /*
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(FirstMenuButton);
        */

        Screen.fullScreen = true;

        Elevator.position = Spots[targetSpot].position;
        
        chapterSelector.Init();
    }

    private void Update()
    {
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

    public void ChangeSpots(int index)
    {
        if (!CanInput)
            return;

        targetSpot = index;
        moving = true;
    }

    public void UpdateVolume()
    {
        if (PersistentData.Instance != null)
            PersistentData.Instance.MasterVolume = masterVolume.value;
    }

    public void StartGame()
    {
        if (!CanInput)
            return;

        CanInput = false;
        eventSystem.SetActive(false);

        StartCoroutine(C_StartGame());
    }

    IEnumerator C_StartGame()
    {
        BlackFadeTo(1, 4f);

        yield return new WaitForSeconds(9f);

        emitter.Stop();

        yield return new WaitForSeconds(silenceTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
