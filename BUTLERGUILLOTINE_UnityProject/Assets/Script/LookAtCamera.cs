using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] float offsetExperimental;

    Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        transform.up = cam.up;
        transform.right = cam.right;

        var forward = Quaternion.AngleAxis(offsetExperimental, Vector3.up) * cam.forward;
        transform.forward = forward;
    }
}
