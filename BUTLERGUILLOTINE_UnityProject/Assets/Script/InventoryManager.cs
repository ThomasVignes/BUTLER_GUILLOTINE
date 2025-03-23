using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Item[] items;
    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] GameObject canvas;

    public void Init(Item[] items)
    {
        this.items = new Item[items.Length];

        for (int i = 0; i < this.items.Length; i++)
        {
            var it = items[i];

            this.items[i] = new Item(it.ID, it.Name, it.Sprite, it.LimitedUses, it.Uses);
        }
    }

    public void HideCanvas(bool hidden)
    {
        canvas.SetActive(!hidden);
    }

    public void UseItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.Equipped)
            return;

        if (!item.LimitedUses)
            return;

        item.Uses--;

        if (item.Uses <= 0)
        {
            Destroy(item.Icon);
            item.Equipped = false;
        }
    }

    public void EquipItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || item.Equipped)
            return;

        GameObject go = Instantiate(iconPrefab, iconParent);
        go.GetComponent<Image>().sprite = item.Sprite;
        go.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
        item.Icon = go;
        item.Equipped = true;
    }

    public void RemoveItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.Equipped)
            return;

        Destroy(item.Icon);
        item.Equipped = false;
    }
}

[System.Serializable]
public class Item
{
    public string ID;
    public string Name;
    public Sprite Sprite;
    public bool LimitedUses;
    public int Uses;
    [HideInInspector] public bool Equipped;
    [HideInInspector] public GameObject Icon;

    public Item(string id, string name, Sprite sprite, bool limitedUses, int uses)
    {
        ID = id;
        Name = name;
        Sprite = sprite;
        LimitedUses = limitedUses;
        Uses = uses;
    }
}
