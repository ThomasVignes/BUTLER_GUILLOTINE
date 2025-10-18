using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickMaterialEmissive : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [ColorUsage(true, true)]
    [SerializeField] Color baseEmissive, targetEmissive;

    MeshRenderer meshRenderer;

    float timer;

    bool descending;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (descending)
        {
            if (timer < 0)
            {
                timer = 0;
                descending = false;
            }
            else
                timer -= Time.deltaTime * speed;

        }
        else
        {
            if (timer > 1)
            {
                timer = 1;
                descending = true;
            }
            else
                timer += Time.deltaTime * speed;
        }

        ChangeColor();
    }

    void ChangeColor()
    {
        Color newColor = Color.Lerp(baseEmissive, targetEmissive, timer);

        meshRenderer.material.SetColor("_EmissionColor", newColor);
    }
}
