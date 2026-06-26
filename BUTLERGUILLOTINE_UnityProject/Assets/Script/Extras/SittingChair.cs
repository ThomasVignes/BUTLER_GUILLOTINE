using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingChair : MonoBehaviour
{
    public float speed = 0.1f;
    private float pitch, yaw;
    public float pitchMin, pitchMax;    
    public float yawMin, yawMax;    


    [SerializeField] Transform rotator;

    bool canMove;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Activate(true);

        if (!canMove)
            return;

        pitch += Input.GetAxis("Mouse Y") * speed;
        yaw += Input.GetAxis("Mouse X") * speed;

        // Clamp pitch to avoid issues and sign flips at +/- 90
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        yaw = Mathf.Clamp(yaw, yawMin, yawMax);

        var phi = pitch * Mathf.Deg2Rad;
        var theta = yaw * Mathf.Deg2Rad;

        var sinTheta = Mathf.Sin(theta);
        var cosTheta = Mathf.Cos(theta);
        var sinPhi = Mathf.Sin(phi);
        var cosPhi = Mathf.Cos(phi);

        // Convert from spherical to cartesian and directly assign to up. Simple but requires clamping pitch
        var fwd = new Vector3(cosPhi * sinTheta, sinPhi, cosPhi * cosTheta);
        rotator.transform.forward = fwd;

        // Alternatively, if you want pitch to be able to exceed 90/-90 without freaking out, remove the above transform.forward call and calculate the spherical up vector directly
        var up = new Vector3(-sinPhi * sinTheta, cosPhi, -sinPhi * cosTheta);
        rotator.transform.localRotation = Quaternion.LookRotation(fwd, up);
    }

    public void Activate(bool active)
    {
        GameManager.Instance.SetCinematicMode(active, false);

        if (active)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        StartCoroutine(C_Transition(active));
    }

    IEnumerator C_Transition(bool active)
    {
        canMove = !active;
        GameManager.Instance.ScreenEffects.FadeTo(1, 1f);

        yield return new WaitForSeconds(1f);

        rotator.gameObject.SetActive(active);
        GameManager.Instance.ScreenEffects.FadeTo(0, 1f);

        yield return new WaitForSeconds(1f);

        canMove = active;
    }
}
