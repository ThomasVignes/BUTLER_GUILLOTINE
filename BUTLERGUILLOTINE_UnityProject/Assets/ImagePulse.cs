using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePulse : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float pulseSpeed;
    [SerializeField] float minOpacity, maxOpacity;
    [SerializeField] float fadeOutSpeed;

    [Header("References")]
    [SerializeField] Image image;

    float currentA;

    bool rising, fadeOut;

    private void Awake()
    {
        currentA = minOpacity;
        rising = true;
    }

    private void Update()
    {
        if (fadeOut)
        {

            if (currentA > 0)
                currentA -= fadeOutSpeed * Time.deltaTime;
            else
                currentA = 0;

            return;
        }

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

    public void FadeOut()
    {
        fadeOut = true;
    }
}
