using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    [Header("General")]
    public string ID;
    public string Name;
    public Sprite Sprite;
    public bool LimitedUses;
    public int Uses;
    [HideInInspector] public bool Equipped;
    [HideInInspector] public GameObject Icon;

    [Header("Inventory")]
    public InspectLine[] Inspect;
    public GameObject InteractableObject;
}

[System.Serializable]
public class InspectLine
{
    public string Text;
    public float Duration;
}
