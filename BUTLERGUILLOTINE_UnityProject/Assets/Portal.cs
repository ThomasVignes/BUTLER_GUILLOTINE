using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Phase shifting")]
    [SerializeField] PortalDoor portalDoor;
    [SerializeField] PortalDoor linkedDoor;
    [SerializeField] PlayerSwapper swapper;

    [Header("Toggling")]
    [SerializeField] GameObject body;
    [SerializeField] GameObject door;
    public Transform spot;

    bool open;

    private void Start()
    {
        body.SetActive(!open);
        door.SetActive(open);
    }

    public void Toggle(bool open)
    {
        if (this.open == open)
            return;

        this.open = open;

        StartCoroutine(C_Toggle(open));
    }

    IEnumerator C_Toggle(bool open)
    {
        GameManager.Instance.ScreenEffects.WhiteFadeInOut(0.1f, 1f, 0.2f);

        yield return new WaitForSeconds(0.15f);

        body.SetActive(!open);
        door.SetActive(open);

        if (open)
            portalDoor.Init(linkedDoor, swapper, false);
    }
}
