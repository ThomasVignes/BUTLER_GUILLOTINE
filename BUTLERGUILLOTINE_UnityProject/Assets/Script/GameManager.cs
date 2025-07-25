using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Whumpus;
using FMOD;
using FMODUnity;
using System.Net.Http;
using UnityEngine.Events;

[System.Serializable]
public class Conditions
{
    public string Name;
    public bool Met;

    public Conditions(string Name,  bool Met)
    {
        this.Name = Name;
        this.Met = Met;
    }
}

[System.Serializable]
public class Area
{
    public string Name;
    public AudioSource Music;
    public AudioSource CopyrightFree;
    public EventReference Track;

    public float StepsVolume;
    public bool ImmuneExperimental;
    [HideInInspector] public float OriginalVolume;

    public void Init()
    {
        if (Music == null)
            return;

        if (PersistentData.Instance != null && PersistentData.Instance.CopyrightFree)
        {
            Music = CopyrightFree;
            ImmuneExperimental = true;
        }

        OriginalVolume = Music.volume;
    }

    public Area (string name, EventReference track, float stepsVolume, bool immuneExperimental)
    {
        Name = name;
        Track = track;
        StepsVolume = stepsVolume;
        ImmuneExperimental = immuneExperimental;
    }
}

[System.Serializable]
public class PlayableCharacter
{
    public string ID;
    public CharacterData Data;
    public GameObject Instance;

    public PlayableCharacter(string ID, CharacterData Data)
    {
        this.ID = ID;
        this.Data = Data;
    }
}

public class GameManager : MonoBehaviour
{
    //Global Hidden Values
    public float StrongPunctuationWait = 0.4f;
    public float LightPunctuationWait = 0.07f;

    public static GameManager Instance;

    [Header("Chapter Data")]
    public ChapterData ChapterData;
    public string StartCinematic;
    public List<PlayableCharacter> PlayableCharacters = new List<PlayableCharacter>();

    [Header("Scene Settings")]
    public bool Paused;
    public bool SpecialActive;
    public bool ManualPlayerSpawn;
    public bool LockSpecial;
    public string LockSpecialComment;

    [Header("Clicking")]
    [SerializeField] private float clickdelay;
    [SerializeField] private LayerMask moveLayer, interactLayer, wallLayer, ignoreLayers, hitboxLayer;

    [Header("Cameras")]
    [SerializeField] private GameObject currentCam;
    [SerializeField] private GameObject vnCam;
    [SerializeField] private CameraZone firstCamZone;
    private Character character;

    [Header("Inventory")]
    [SerializeField] UnityEvent OnInventoryOpen;
    [SerializeField] UnityEvent OnInventoryClose;

    [Header("References")]
    [SerializeField] PlayerFollower playerFollower;
    [SerializeField] ChapterManagerGeneric startGameManager;
    [SerializeField] private Transform characterStart;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] ThemeManager themeManager;
    [SerializeField] Manager[] genericManagers;
    public PauseManager PauseManager;


    public List<Conditions> conditions = new List<Conditions>();

    PlayerController player;

    Vector3 startPos;
    Quaternion startRot;
    private List<Character> characters = new List<Character>();
    private int clicked;
    private float clicktime;
    private bool specialActive;


    private CameraZone currentCamZone;
    private CursorManager cursorManager;
    private GhostManager ghostManager;

    [HideInInspector] public CinematicManager CinematicManager;
    [HideInInspector] public DialogueManager DialogueManager;
    [HideInInspector] public ScreenEffects ScreenEffects;
    [HideInInspector] public HitstopManager HitstopManager;
    [HideInInspector] public InventoryManager InventoryManager;
    [HideInInspector] public CameraEffectManager CameraEffectManager;
    [HideInInspector] public PartnerManager PartnerManager;

    public LayerMask MoveLayer { get { return moveLayer; } }
    public LayerMask IgnoreLayers { get { return ignoreLayers; } }
    public LayerMask InteractLayer { get { return interactLayer; }}
    public LayerMask WallLayer { get { return wallLayer; }}
    public bool CinematicMode { get { return cinematicMode; } }
    public bool InventoryMode { get { return inventoryMode; } set { inventoryMode = value; } }
    public bool VNMode { get { return vnMode; } }
    public bool Ready { get { return ready; } set { ready = value; } }
    public bool DialogueCinematic { get { return dialogueCinematic;} set {  dialogueCinematic = value; } }
    public bool End { get { return end; } set { end = value; } }
    public PlayerController Player { get { return player; } }
    public PlayerFollower PlayerFollower { get { return playerFollower; } }
    public CursorManager CursorManager { get { return cursorManager; } }
    public ThemeManager ThemeManager { get { return themeManager; } }
    public string EquippedItemID { get { return InventoryManager.EquippedItemID; } }

    [HideInInspector] public Action CameraTick;


    bool cinematicMode, vnMode, commentMode, end, overrideAmbiance, ready, dialogueCinematic;
    bool cinematicStart, inventoryMode;
    bool canInventory;

    private void Awake()
    {
        Instance = this;

        if (playerFollower == null)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("GameManagement/PlayerFollower"));
            playerFollower = go.GetComponent<PlayerFollower>();
        }

        if (PersistentData.Instance == null)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("GameManagement/_PersistentData"));
            go.GetComponent<PersistentData>().QuickInit();
        }

        PersistentData.Instance.ResetMultipliers();


        PlayableCharacters.Clear();

        var startCharacter = new PlayableCharacter(ChapterData.StartCharacter.Name, ChapterData.StartCharacter);

        PlayableCharacters.Add(startCharacter);

        if (ChapterData.OtherCharacters.Length > 0)
        {
            foreach (var characters in ChapterData.OtherCharacters)
            {
                PlayableCharacters.Add(new PlayableCharacter(characters.Name, characters));
            }
        }

        var instance = InstantiatePlayer(startCharacter, true);

        PauseManager.Init(this);
        themeManager.Init();

        PartnerManager = GetComponent<PartnerManager>();
        PartnerManager.Init(this);

        ScreenEffects = GetComponent<ScreenEffects>();

        cursorManager = GetComponent<CursorManager>();
        cursorManager.Init();

        DialogueManager = FindObjectOfType<DialogueManager>();
        DialogueManager.Init(this);

        CinematicManager = FindObjectOfType<CinematicManager>();
        CinematicManager.Init(this);

        ghostManager = FindObjectOfType<GhostManager>();

        ghostManager.UpdateGhosts();

        HitstopManager = FindObjectOfType<HitstopManager>();

        InventoryManager = FindObjectOfType<InventoryManager>();

        CameraEffectManager = GetComponent<CameraEffectManager>();

        CameraEffectManager.Init(this);

        Character[] chars = FindObjectsOfType<Character>();

        foreach (Character c in chars)
        {
            if (!(c is PlayerController))
            {
                c.Init();
                characters.Add(c);
            }
        }

        currentCamZone = firstCamZone;


        foreach(var c in ChapterData.conditions)
        {
            conditions.Add(new Conditions(c.Name, c.Met));
        }

        //areas = ChapterData.areas;
        InventoryManager.Init(this, ChapterData.items);

        canInventory = true;

        foreach (var item in genericManagers)
        {
            item.Init(this);
        }



        if (StartCinematic != "")
        {
            CinematicManager.PlayCinematic(StartCinematic);

            cinematicStart = true;
        }
        else
        {
            if (startGameManager != null)
            {
                startGameManager.Init(this);

                startPos = player.transform.position;
                startRot = player.transform.rotation;

                startGameManager.StartGame();
            }
            else
                ready = true;
        }
    }

    public void SwapPlayer(string playerName)
    {
        SwapPlayer(playerName, false);
    }

    public bool SwapPlayer(string playerName, bool hidePreviousPlayer)
    {
        bool found = false; ;

        foreach (var item in PlayableCharacters)
        {
            if (item.ID == playerName)
            {
                player.UnInit();

                if (hidePreviousPlayer)
                    HidePlayer(true);

                if (item.Instance == null)
                    InstantiatePlayer(item, true);
                else
                    ControlInstance(item.Instance);

                found = true;
                break;
            }
        }

        return found;
    }

    public bool SwapPlayer(string playerName, Transform newPos, bool hidePreviousPlayer)
    {
        bool found = SwapPlayer(playerName, hidePreviousPlayer);

        TeleportPlayer(newPos);

        return found;
    }

    public void HidePlayer(bool masked)
    {
        player.Hide(masked);
    }

    public void RemoveCharacter(Character character)
    {
        characters.Remove(character);
    }

    public void PlayerReady()
    { 
        player.Ready();
    }

    public void PausePlayerPath()
    {
        player.Pause();
    }

    public void CanInventoryToggle(bool active)
    {
        canInventory = active;
    }

    private void Update()
    {
        PauseManager.CanPause = !cinematicMode && !cinematicStart && ready;

        PauseManager.Step();

        if (Paused)
        {
            cursorManager.SetCursorType(CursorType.Base);
            return;
        }

        foreach (var item in genericManagers)
        {
            item.Step();
        }

        if (cinematicMode)
        {
            CinematicManager.Step();
            cursorManager.SetCursorType(CursorType.Invisible);

            if (Input.GetKeyDown(KeyCode.RightShift))
                CinematicManager.SkipCinematic();

            foreach (Character c in characters)
            {
                if (c.CanMoveInCinematic)
                {
                    c.Step();
                    c.ConstantStep();
                }
            }
            return;
        }

        if (cinematicStart)
        {
            if (startGameManager != null)
            {
                startGameManager.Init(this);

                startPos = player.transform.position;
                startRot = player.transform.rotation;

                startGameManager.StartGame();

                cinematicStart = false;
            }
            else
                ready = true;
        }

        if (!ready || end)
        {
            if (end)
                cursorManager.SetCursorType(CursorType.Invisible);
            else
                cursorManager.SetCursorType(CursorType.Base);

            return;
        }

        player.ConstantStep();
        PartnerManager.ConstantStep();

        foreach (Character c in characters)
        {
            c.ConstantStep();
        }

        if (startGameManager != null && startGameManager.Intro)
        {
            cursorManager.SetCursorType(CursorType.Base);

            startGameManager.IntroStep();

            return;
        }

        if (!dialogueCinematic)
        {
            if (inventoryMode)
            {
                CursorHover();
                player.InventoryController.UpdateInventory();

                if (Input.GetButtonDown("Pause"))
                {
                    player.InventoryController.SetActive(false, true);

                    OnInventoryClose?.Invoke();
                }

                return;
            }
            else
            {
                if (Input.GetButtonDown("Pause") && canInventory)
                {
                    player.Pause();
                    player.InventoryController.SetActive(true, true);

                    InventoryManager.InstaHideNotification();

                    OnInventoryOpen?.Invoke();
                }
            }
        }


        if (commentMode)
        {
            cursorManager.SetCursorType(CursorType.Base);
            DialogueManager.Step();
            return;
        }

        if (vnMode)
        {
            cursorManager.SetCursorType(CursorType.Base);
            DialogueManager.Step();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryClick();

            player.ToggleRun(HandleDoubleClick());
        }

        if (SpecialActive)
        {
            if (Input.GetMouseButton(1) && !specialActive)
            {
                player.ToggleSpecial(true);

                specialActive = true;
            }

            if (!Input.GetMouseButton(1) && specialActive)
            {
                player.ToggleSpecial(false);

                specialActive = false;
            }
        }

        player.Step();
        PartnerManager.Step();

        foreach (Character c in characters)
        {
            c.Step();
        }

        CursorHover();
    }

    GameObject InstantiatePlayer(PlayableCharacter playableCharacter, bool control)
    {
        return InstantiatePlayer(playableCharacter, characterStart, control);
    }

    GameObject InstantiatePlayer(PlayableCharacter playableCharacter, Transform overridePos, bool control)
    {
        GameObject chara = Instantiate(playableCharacter.Data.ControllerPrefab, overridePos.position, overridePos.rotation);
        playableCharacter.Instance = chara;

        if (control)
            ControlInstance(chara);

        

        return chara;
    }

    public void PreInstantiatePlayer(string Name, Transform overridePos)
    {
        PlayableCharacter character = null;

        foreach (PlayableCharacter c in PlayableCharacters)
        {
            if (c.ID == Name)
            {
                character = c;
                break;
            }
        }

        InstantiatePlayer(character, overridePos, false);
    }

    void ControlInstance(GameObject instance)
    {
        character = instance.GetComponentInChildren<Character>();
        player = instance.GetComponentInChildren<PlayerController>();

        if (!player.Initialized)
            player.Init();

        playerFollower.SetTarget(player);

        HidePlayer(false);
    }

    public void ToggleInventoryMode(bool active)
    {
        DialogueManager.ToggleCanvas(!active);
        inventoryMode = active;
    }

    public void SetPartner(Character character)
    {
        PartnerManager.SetPartner(character);
    }

    public void InjurePlayer(bool injure)
    {
        player.Injure(injure);
    }


    public void EndChapter()
    {
        end = true;

        startGameManager.EndChapter();
    }

    public void EndGame(string message)
    {
        startGameManager.Death(message);
    }


    private bool HandleDoubleClick()
    {
        /*
        if (player.Running)
        {
            if (Time.time - clicktime < clickdelay)
            {

            }
        }
        */

        if (Time.time - clicktime > clickdelay)
            clicked = 0;

        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            //clicked = 0;
            //clicktime = 0;

            clicktime = Time.time;
            return true;
        }
        else if (Time.time - clicktime > clickdelay)
        {
            clicked = 0;
            clicktime = 0;
        }

        return false;
    }

    private void TryClick()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (player.SpecialMode)
        {
            if (LockSpecial)
            {
                WriteComment(LockSpecialComment);
                player.Pause();
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitboxLayer))
            {
                player.Special(hit.point, hit.transform.gameObject);
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            if (player.SpecialMode)
            {
                player.Special(hit.point, hit.transform.gameObject);
            }
            else
            {
                if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
                {
                    return;
                }

                Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

                if (interactable != null)
                {
                    player.SetDestination(interactable.GetTargetPosition(), interactable);
                    interactable.OnSelected?.Invoke();

                    if (interactable is PickupInteractable)
                    {
                        player.PickUpAnim();
                    }

                    return;
                }

                if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer))
                {
                    player.SetDestination(hit.point);
                    return;
                }
            }
        }
        
    }

    public void PlayerMoveTo(Transform spot)
    {
        PlayerMoveTo(spot.position);
    }

    public void PlayerMoveTo(Vector3 spot)
    {
        player.SetDestination(spot);
    }

    private void CursorHover()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            if (player.SpecialMode)
            {
                cursorManager.SetCursorType(player.CursorType);
                return;
            }

            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
            {
                cursorManager.SetCursorType(CursorType.Base);
                return;
            }

            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                cursorManager.SetCursorType(CursorType.Look);

                return;
            }

            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer))
            {
                cursorManager.SetCursorType(CursorType.Move);
                return;
            }
        }
    }

    public void SetVNMode(bool yes, bool noCam)
    {
        vnMode = yes;

        //currentCam.SetActive(!yes);
        if (noCam)
            vnCam.SetActive(false);
        else
            vnCam.SetActive(yes);

        currentCamZone.active = !yes;

        if (!cinematicMode)
            inventoryCanvas.SetActive(!yes);

        if (!yes)
            ghostManager.UpdateGhosts();
    }

    public void SetCinematicMode(bool yes, bool noBounce)
    {
        cinematicMode = yes;

        if (inventoryMode)
        {
            ToggleInventoryMode(false);
        }

        currentCamZone.active = !yes;

        if (!vnMode)
            inventoryCanvas.SetActive(!yes);

        if (!yes)
            ghostManager.UpdateGhosts();

        if (BounceLight.Instance != null && noBounce)
            BounceLight.Instance.Toggle(!yes);
    }

    CommentInteractable currentComment;

    public void WriteComment(string text)
    {
        if (commentMode)
            return;

        commentMode = true;

        //string comment = this.ChapterData.CommentData.GetCommentWithID(text);

        DialogueManager.WriteSpecific(text);
    }

    public void WriteComment(string text, CommentInteractable comment)
    {
        if (commentMode)
            return;

        currentComment = comment;
        currentComment.ToggleCommenting(true);

        WriteComment(text);
    }

    public void EndComment()
    {
        if (!commentMode)
            return;

        commentMode = false;

        if (currentComment != null)
        {
            currentComment.ToggleCommenting(false);
            currentComment = null;
        }
    }

    public void MoveStart(Transform target)
    {
        characterStart.position = target.position;
        characterStart.rotation = target.rotation;

        startPos = player.transform.position;
        startRot = player.transform.rotation;
    }

    public void NewArea(string areaName)
    {
        themeManager.NewArea(areaName);
    }

    public void NewArea(string areaName, float volume)
    {
        themeManager.NewArea(areaName, volume);
    }

    public void StopOverride()
    {
        themeManager.StopOverride();
    }

    public void StopOverride(string  areaName)
    {
        themeManager.StopOverride(areaName);
    }

    public void ResumeAmbiance()
    {
        themeManager.ResumeAmbiance(); 
    }

    public void StopAmbiance()
    {
        themeManager.StopAmbiance();
    }

    public void PlayEndAmbiance()
    {
        themeManager.PlayEndAmbiance();
    }

    public void CamZoneQuickUpdate(CameraZone zone)
    {
        currentCamZone = zone;
    }

    public void SetCamZone(CameraZone zone)
    {
        //Safety
        if (currentCamZone == null)
            currentCamZone = firstCamZone;

        if (zone == null)
            return;

        //Update zone specific objects
        List<GameObject> previous = new List<GameObject>();

        if (currentCam != null)
            previous = currentCamZone.ShotSpecificObjects;

        List<GameObject> next = zone.ShotSpecificObjects;
        foreach (var item in previous)
        {
            if (!next.Contains(item))
            {
                RagdollHider hider = item.GetComponent<RagdollHider>();
                if (hider != null)
                {
                    hider.Hide();
                }
                else
                {
                    if (item.activeSelf)
                        item.SetActive(false);
                }
            }
        }

        foreach (var item in next)
        {
            RagdollHider hider = item.GetComponent<RagdollHider>();
            if (hider != null)
            {
                hider.Show();
            }
            else
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }
        }

        //Update zone hidden objects
        previous = currentCamZone.ShotSpecificHide;
        next = zone.ShotSpecificHide;
        foreach (var item in previous)
        {
            if (!next.Contains(item))
            {
                if (item != null)
                {
                    RagdollHider hider = item.GetComponent<RagdollHider>();
                    if (hider != null)
                    {
                        hider.Show();
                    }
                    else
                    {
                        if (!item.activeSelf)
                            item.SetActive(true);
                    }
                }
            }
        }

        foreach(var item in next)
        {
            if (item != null)
            {
                RagdollHider hider = item.GetComponent<RagdollHider>();
                if (hider != null)
                {
                    hider.Hide();
                }
                else
                {
                    if (item.activeSelf)
                        item.SetActive(false);
                }
            }
        }

        CameraTick?.Invoke();

        currentCamZone = zone;

        cursorManager.ToggleBlackAndWhite(currentCamZone.BlackAndWhite);
    }

    public void UpdateCondition(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                c.Met = true;
        }
    }

    public void FalseCondition(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                c.Met = false;
        }
    }

    public bool ConditionMet(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                return c.Met;
        }

        return false;
    }

    public void OverrideAmbiance(string overrideArea)
    {
        themeManager.OverrideAmbiance(overrideArea);
    }

    public void SetAmbianceVolume(float sound)
    {
        themeManager.SetAmbianceVolume(sound);
    }

    [ContextMenu("Unlock all doors")]
    public void UnlockAllDoors()
    {
        Door[] doors = FindObjectsOfType<Door>();

        foreach (var item in doors)
        {
            item.ToggleDoorNoEvent(true);
        }
    }

    public void ResetPlayer()
    {
        player.transform.position = startPos;
        player.transform.rotation = startRot;
        player.ResetState();
    }

    public void TeleportPlayer(Vector3 pos, Quaternion rot)
    {
        player.Freeze(true);
        player.transform.position = pos;
        player.transform.rotation = rot;
        player.Freeze(false);
    }

    public void TeleportPlayer(Transform transform)
    {
        TeleportPlayer(transform.position, transform.rotation);
    }

    public void TeleportPlayer(Transform transform, float delay, bool blackscreen)
    {
        StartCoroutine(C_TeleportPlayer(transform, delay, blackscreen));
    }

    public void TeleportPlayerPresetDelay(Transform transform)
    {
        TeleportPlayer(transform, 2, true);
    }

    IEnumerator C_TeleportPlayer(Transform transform, float delay, bool blackscreen)
    {
        if (blackscreen)
            ScreenEffects.FadeTo(1, delay / 2);

        yield return new WaitForSeconds(delay/2);

        TeleportPlayer(transform);

        yield return new WaitForSeconds(1);

        if (blackscreen)
            ScreenEffects.FadeTo(0, delay / 2);

        yield return new WaitForSeconds(delay / 2);
    }

    public void ChopLimb(string ID)
    {
        foreach (var item in player.Choppers)
        {
            if (item.ID == ID)
                item.Chop();
        }
    }

    public void PutMask(bool on)
    {
        MaskManager maskManager = player.transform.GetComponent<MaskManager>();

        if (maskManager == null)
            player.transform.GetComponentInChildren<MaskManager>();

        if (maskManager != null)
            maskManager.PutMask(on);

        if (PartnerManager.Partner != null)
        {
            maskManager = PartnerManager.Partner.transform.GetComponent<MaskManager>();

            if (maskManager == null)
                PartnerManager.Partner.transform.GetComponentInChildren<MaskManager>();

            if (maskManager != null)
                maskManager.PutMask(on);
        }
    }

    public void ExperimentalDeactivatePlayer()
    {
        player.transform.root.gameObject.SetActive(false);

        if (PartnerManager.Partner != null)
        {
            PartnerManager.Partner.transform.root.gameObject.SetActive(false);
        }
    }

    public void TryRealityShift(string code)
    {
        
    }
}
