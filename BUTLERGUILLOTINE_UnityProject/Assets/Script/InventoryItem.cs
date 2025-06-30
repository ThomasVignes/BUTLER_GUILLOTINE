using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : Interactable
{
    [SerializeField] ItemData data;

    public InspectLine[] Inspect { get { return data.Inspect; } }
    public ItemData Data { get { return data; } }

    public void Init(ItemData d)
    {
        data = d;
    }
}