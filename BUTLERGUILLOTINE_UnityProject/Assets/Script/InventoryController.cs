using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public bool Standalone;
    public bool Active;
    [SerializeField] float spotlightLerp, ughDist, activateDelay;
    [SerializeField] LayerMask itemLayer, spotlightLayers;
    [SerializeField] Transform vcam, spotlight, head;
    [SerializeField] Animator characterAnimator, curtains;
    [SerializeField] GameObject lights, cam;
    [SerializeField] Image blackScreen;
    [SerializeField] ItemSpot[] itemSpots;
    [SerializeField] ItemData[] startItems;

    [Header("Writing")]
    [SerializeField] float delayBetweenLetters;
    [SerializeField] GameObject textUI;
    [SerializeField] TextMeshProUGUI dialogue;

    float strongPuncWait, lightPuncWait;

    Quaternion originalRotation, lookRotation;

    Coroutine activeDelay, examine;

    GameManager gameManager;

    bool speaking, skip;

    InventoryItem selectedItem, equippedItem;

    private void Awake()
    {
        originalRotation = transform.rotation;
        textUI.SetActive(false);

        foreach (var item in startItems)
        {
            AddItem(item);
        }

        if (Standalone)
            SetActive(true, true);
        else
            SetActive(false, false);

        foreach (var item in itemSpots)
        {
            item.ItemUI.Init(this);
        }
    }
    private void Start()
    {
        strongPuncWait = GameManager.Instance.StrongPunctuationWait;
        lightPuncWait = GameManager.Instance.LightPunctuationWait;
    }

    public void Init(GameManager gm)
    {
        Standalone = false;
        gameManager = gm;
        
    }

    private void Update()
    {
        if (Standalone)
            UpdateInventory();
    }

    public void SetActive(bool active, bool blackscreen)
    {
        if (speaking)
        {
            if (examine != null)
                StopCoroutine(examine);

            EndInspect();
        }
        else
        {
            UnselectCurrent();
        }

        if (active)
        {
            if (!Standalone)
                GameManager.Instance.ToggleInventoryMode(true);
        }

        curtains.SetBool("Open", active);

        if (activeDelay != null)
            StopCoroutine(activeDelay);

        activeDelay = StartCoroutine(C_ActiveDelay(active, blackscreen));
    }

    IEnumerator C_ActiveDelay(bool active, bool blackscreen)
    {
        blackScreen.DOFade(1, activateDelay / 2);

        yield return new WaitForSeconds(activateDelay/2);

        cam.SetActive(active);
        lights.SetActive(active);

        blackScreen.DOFade(0, activateDelay / 2);

        yield return new WaitForSeconds(activateDelay / 2);

        if (!active)
        {
            if (!Standalone)
                GameManager.Instance.ToggleInventoryMode(false);
        }

        Active = active;
    }

    public void UpdateInventory()
    {
        if (!Active)
            return;

        if (speaking)
        {
            if (!skip)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!waitingForLine)
                        skip = true;
                    else
                        SkipInspect();
                }
            }

            return;
        }

        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, spotlightLayers))
        {
            lookRotation = Quaternion.LookRotation(hit.point - spotlight.position);
        }
        else
        {
            lookRotation = originalRotation;
        }

        spotlight.rotation = Quaternion.Lerp(spotlight.rotation, lookRotation, spotlightLerp * Time.deltaTime);

        //characterAnimator.SetBool("Ugh", Vector3.Distance(new Vector3(head.position.x, head.position.y, hit.point.z), hit.point) < ughDist);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, itemLayer))
            {
                InventoryItem item = hit.transform.gameObject.GetComponent<InventoryItem>();

                if (item != null)
                {
                    if (selectedItem == null || selectedItem != item)
                        Select(item);
                    else
                        UnselectCurrent();
                }
            }
        }
    }

    void UnselectCurrent()
    {
        if (selectedItem == null)
            return;

        selectedItem.ItemSpot.ItemUI.Show(false);
        selectedItem = null;
    }

    public void Select(InventoryItem item)
    {
        UnselectCurrent();

        selectedItem = item;

        item.ItemSpot.ItemUI.Show(true);
    }

    public void EquipItem()
    {
        if (equippedItem != null)
        {
            if (selectedItem == equippedItem)
            {
                UnequipSelected();
                return;
            }
            else
            {
                GameManager.Instance.InventoryManager.Unequip(equippedItem);
                equippedItem.ItemSpot.ItemUI.UpdateEquipText(false);
            }
        }

        EquipSelected();
    }

    void EquipSelected()
    {
        GameManager.Instance.InventoryManager.Equip(selectedItem);
        selectedItem.ItemSpot.ItemUI.UpdateEquipText(true);

        equippedItem = selectedItem;

    }

    void UnequipSelected()
    {
        GameManager.Instance.InventoryManager.Unequip(selectedItem);
        selectedItem.ItemSpot.ItemUI.UpdateEquipText(false);

        equippedItem = null;
    }

    public void ExamineSelected()
    {
        if (speaking)
            return;

        if (selectedItem == null)
            return;

        textUI.SetActive(true);
        characterAnimator.SetBool("Speak", true);

        if (examine != null)
            StopCoroutine(examine);

        examine = StartCoroutine(C_Inspect(selectedItem.Inspect));
    }

    public void EndInspect()
    {
        if (!speaking)
            return;

        textUI.SetActive(false);
        characterAnimator.SetBool("Speak", false);
        speaking = false;
    }

    bool waitingForLine;
    int lineIndex;
    Coroutine writeLine;

    InspectLine[] lines;

    IEnumerator C_Inspect(InspectLine[] l)
    {
        UnselectCurrent();

        dialogue.text = "";

        lines = l;
        lineIndex = 0;

        yield return new WaitForSeconds(0.1f);

        speaking = true;


        if (lineIndex < lines.Length)
            writeLine = StartCoroutine(C_WriteLine(lines[lineIndex]));
        else
            EndInspect();
    }

    void SkipInspect()
    {
        /*
        if (skip)
            return;
        */

        waitingForLine = false;

        StopCoroutine(writeLine);

        lineIndex++;

        if (lineIndex < lines.Length)
            writeLine = StartCoroutine(C_WriteLine(lines[lineIndex]));
        else
            EndInspect();
    }

    IEnumerator C_WriteLine(InspectLine line)
    {
        skip = false;

        var text = line.Text;

        dialogue.text = "";

        char last = 'a';

        var charCount = 0;

        foreach (char c in text)
        {
            dialogue.text += c;

            EffectsManager.Instance.audioManager.Play("SmallClick");

            charCount++;

            string strongPunctuations = ".?!";
            string lightPunctuations = ",:";

            if (strongPunctuations.Contains(c) && charCount < line.Text.Length - 1)
                yield return new WaitForSeconds(strongPuncWait);
            else if (lightPunctuations.Contains(c))
                yield return new WaitForSeconds(lightPuncWait);
            else
                yield return new WaitForSeconds(delayBetweenLetters);

            //yield return new WaitForSeconds(delayBetweenLetters);

            last = c;

            if (skip)
            {
                skip = false;
                dialogue.text = text;
                break;
            }
        }


        waitingForLine = true;

        yield return new WaitForSeconds(line.Duration);

        waitingForLine = false;

        
        lineIndex++;

        if (lineIndex < lines.Length)
            writeLine = StartCoroutine(C_WriteLine(lines[lineIndex]));
        else
            EndInspect();
    }

    public void AddItem(ItemData data)
    {
        ItemSpot spot = null;

        foreach (var item in itemSpots)
        {
            if (!item.Occupied)
            {
                spot = item;
                break;
            }
        }

        if (spot == null)
            return;

        GameObject itemInteractable = Instantiate(data.InteractableObject);

        itemInteractable.transform.SetParent(spot.Spot);
        itemInteractable.transform.localPosition = Vector3.zero;
        itemInteractable.transform.localRotation = Quaternion.identity;

        itemInteractable.GetComponent<InventoryItem>().Init(data, spot);

        spot.Instance = itemInteractable.GetComponent<InventoryItem>();
        spot.Occupied = true;
        spot.ItemUI.UpdateName(data.Name);
        spot.ItemUI.ToggleEquippable(data.Equippable);
    }

    public void RemoveItem(ItemData data) 
    {
        foreach (var item in itemSpots)
        {
            if (item.Occupied && item.Instance.Data == data)
            {
                Destroy(item.Instance.gameObject);
                item.Occupied = false;
            }
        }
    }
}

[System.Serializable]
public class ItemSpot
{
    public bool Occupied;
    public Transform Spot;
    public InventoryItem Instance;
    public ItemUI ItemUI;
}
