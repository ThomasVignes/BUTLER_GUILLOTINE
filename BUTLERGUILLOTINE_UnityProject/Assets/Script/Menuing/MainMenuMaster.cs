using DG.Tweening;
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

    [Header("References")]
    [SerializeField] private Image BlackScreen;
    [SerializeField] Transform Elevator;
    [SerializeField] Transform[] Spots;

    [Header("MainMenuing")]
    [SerializeField] private GameObject FirstMenuButton;

    [Header("OptionsMenuing")]
    [SerializeField] private GameObject FirstOptionsButton;
    [SerializeField] private Slider masterVolume;

    bool disclaimerMode;
    int targetSpot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BlackFadeTo(1, 0.0001f);
        BlackFadeTo(0, 3f);
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(FirstMenuButton);

        CanInput = true;

        Screen.fullScreen = true;
        AudioListener.pause = false;

        Elevator.position = Spots[targetSpot].position;
    }

    private void Update()
    {
        if (!CanInput)
            return;

        var spot = Spots[targetSpot].position;

        if (Vector3.Distance(Elevator.position, spot) > Time.deltaTime)
            Elevator.position = Vector3.MoveTowards(Elevator.position, spot, travelSpeed * Time.deltaTime);
    }

    public void ChangeSpots(int index)
    {
        if (!CanInput)
            return;

        targetSpot = index;
    }

    public void UpdateVolume()
    {
        AudioListener.volume = masterVolume.value;

        if (PersistentData.Instance != null)
            PersistentData.Instance.Volume = AudioListener.volume;
    }

    public void StartGame()
    {
        if (!CanInput)
            return;

        CanInput = false;

        StartCoroutine(C_StartGame());
    }

    IEnumerator C_StartGame()
    {
        BlackFadeTo(1, 4f);

        yield return new WaitForSeconds(7f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
