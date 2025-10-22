using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DontRaycastTMP : MonoBehaviour
{
    void Start()
    {
        var subMesh = GetComponent<TextMeshProUGUI>();
        subMesh.raycastTarget = false;
    }
}
