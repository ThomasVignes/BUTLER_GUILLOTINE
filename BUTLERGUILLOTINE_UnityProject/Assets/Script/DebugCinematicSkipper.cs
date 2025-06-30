using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugCinematicSkipper : MonoBehaviour
{
    [SerializeField] float showDuration;
    [SerializeField] GameObject skipButton;

    float showTimer;
    Vector3 lastMousePos;

    GameManager gameManager;

    public Vector3 mouseDelta
    {
        get
        {
            return Input.mousePosition - lastMousePos;
        }
    }

    bool moving, lastMoving;

    private void Start()
    {
        skipButton.SetActive(false);

        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (!gameManager.CinematicMode)
            return;

        lastMousePos = Input.mousePosition;

        lastMoving = moving;

        moving = mouseDelta.magnitude > 0.3f || Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1);

        if (moving && !lastMoving)
        {
            skipButton.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;

            showTimer = Time.time + showDuration;
        }


        if (!moving && lastMoving && (showTimer < Time.time)) 
        {
            skipButton.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void EarlyClose()
    {
        gameManager.CinematicManager.SkipCinematic();
        skipButton.SetActive(false);
    }
}
