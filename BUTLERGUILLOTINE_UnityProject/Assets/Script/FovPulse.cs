using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovPulse : MonoBehaviour
{
    public bool Active;

    [Header("Values")]
    [SerializeField] float pulseSpeed;
    [SerializeField] float minBonus;
    [SerializeField] float maxBonus;

    [Header("Break")]
    [SerializeField] Vector3 breakOffset;

    [Header("References")]
    [SerializeField] CinemachineVirtualCamera[] cameras;

    List<float> FOVs = new List<float>();

    float bonusValue;
    bool rising = true;

    public void Activate(bool active)
    {
        Active = active;
    }

    public void Break()
    {
        if (!Active)
            return;

        Active = false;

        foreach (var item in cameras)
        {
            item.transform.localRotation *= Quaternion.Euler(breakOffset);
        }
    }

    private void Start()
    {
        foreach (var item in cameras)
        {
            FOVs.Add(item.m_Lens.FieldOfView);
        }
    }

    private void Update()
    {
        if (!Active)
            return;

        if (rising)
        {
            if (bonusValue < maxBonus)
                bonusValue += Time.deltaTime * pulseSpeed;
            else
                rising = false;
        }
        else
        {
            if (bonusValue > minBonus)
                bonusValue -= Time.deltaTime * pulseSpeed;
            else
                rising = true;
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].m_Lens.FieldOfView = FOVs[i] + bonusValue;
        }
    }
}
