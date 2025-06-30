using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityShifting : MonoBehaviour
{
    [Header("Typing (text in lowercase only please)")]
    [SerializeField] string type;
    [SerializeField] float delayBeforeReset;

    string currentString;
    float timer;

    void Update()
    {
        string typed = Input.inputString;

        if (typed.Length > 0)
        {
            if (Char.IsLetter(typed[0]))
            {
                currentString += Char.ToLower(typed[0]);

                if (currentString[currentString.Length - 1] == type[currentString.Length - 1])
                {
                    timer = delayBeforeReset;
                    Debug.Log(currentString);
                }
                else
                {
                    currentString = "";
                }
            }
        }

        if (currentString == type)
        {
            GameManager.Instance.TryRealityShift(currentString);
            currentString = "";
        }



        timer -= Time.deltaTime;


        if (timer <= 0)
        {
            currentString = "";
        }

    }
}
