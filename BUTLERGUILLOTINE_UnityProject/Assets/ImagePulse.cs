using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePulse : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pulseSpeed;
    [SerializeField] float minOpacity, maxOpacity;

    [Header("References")]
    [SerializeField] Image image;

    float currentA;

    bool rising;

    private void Awake()
    {
        currentA = minOpacity;
        rising = true;
    }

    private void Update()
    {
        if (rising) 
        { 
            if (currentA < maxOpacity)
            {
                currentA += pulseSpeed * Time.deltaTime;
            }
            else
            {
                currentA = maxOpacity;
                rising = false;
            }
        }
        else
        {
            if (currentA > minOpacity) 
            { 
                currentA -= pulseSpeed * Time.deltaTime;
            }
            else
            {
                currentA = minOpacity;
                rising = true;
            }
        }

        Color color = image.color;

        color.a = currentA/100f;

        image.color = color;
    }
}
