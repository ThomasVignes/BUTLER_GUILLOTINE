using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] GameObject body, door;
    public Transform spot;

    bool open;

    public void Toggle(bool open)
    {
        if (this.open == open)
            return;

        this.open = open;

        StartCoroutine(C_Toggle(open));
    }

    IEnumerator C_Toggle(bool open)
    {
        GameManager.Instance.ScreenEffects.WhiteFadeInOut(0.1f, 2f, 0.3f);

        yield return new WaitForSeconds(0.2f);

        body.SetActive(!open);
        door.SetActive(open);
    }
}
