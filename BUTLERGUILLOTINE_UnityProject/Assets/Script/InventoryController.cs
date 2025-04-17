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

    Quaternion originalRotation, lookRotation;

    Coroutine activeDelay, inspect;

    GameManager gameManager;

    bool speaking;

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
            if (inspect != null)
                StopCoroutine(inspect);

            EndInspect();
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
            return;

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
                    Inspect(item);
                }
            }
        }
    }

    public void Inspect(InventoryItem item)
    {
        if (speaking)
            return;

        textUI.SetActive(true);
        characterAnimator.SetBool("Speak", true);
        speaking = true;

        if (inspect != null)
            StopCoroutine(inspect);

        inspect = StartCoroutine(C_Inspect(item.Inspect));
    }

    public void EndInspect()
    {
        if (!speaking)
            return;

        textUI.SetActive(false);
        characterAnimator.SetBool("Speak", false);
        speaking = false;
    }

    IEnumerator C_Inspect(InspectLine[] lines)
    {
        dialogue.text = "";

        yield return new WaitForSeconds(0.1f);

        foreach (var line in lines)
        {
            var text = line.Text;

            dialogue.text = "";

            char last = 'a';

            foreach (char c in text)
            {
                dialogue.text += c;

                EffectsManager.Instance.audioManager.Play("SmallClick");

                yield return new WaitForSeconds(delayBetweenLetters);

                /*
                if (c == '.' && last != c)
                    yield return new WaitForSeconds(delayBetweenLetters);
                */

                last = c;
            }

            yield return new WaitForSeconds(line.Duration);
        }

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

        itemInteractable.GetComponent<InventoryItem>().Init(data);

        spot.Instance = itemInteractable.GetComponent<InventoryItem>();
        spot.Occupied = true;
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
}
