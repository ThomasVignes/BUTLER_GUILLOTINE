using DG.Tweening;
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
    [SerializeField] TextMeshProUGUI notification;

    GameManager gameManager;

    string equippedItemID;

    public string EquippedItemID { get { return equippedItemID; } }

    public void Init(GameManager gm, Item[] items)
    {
        gameManager = gm;

        this.items = new Item[items.Length];

        for (int i = 0; i < this.items.Length; i++)
        {
            var it = items[i];

            this.items[i] = new Item(it.Data);
        }

        notification.DOFade(0, 0.01f);
    }

    public void ShowNotification(string itemName)
    {
        if (itemName == "")
            itemName = "item";

        notification.text = "picked up new item";
        notification.DOFade(1, 0.5f).OnComplete(() => StartCoroutine(StayThenFade()));
    }

    IEnumerator StayThenFade()
    {
        yield return new WaitForSeconds(2);

        notification.DOFade(0, 0.5f);
    }

    public void InstaHideNotification()
    {
        notification.DOFade(0, 0.001f);
    }

    public void HideCanvas(bool hidden)
    {
        canvas.SetActive(!hidden);
    }

    public void UseItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.InInventory)
            return;

        if (!item.LimitedUses)
            return;

        item.Uses--;

        if (item.Uses <= 0)
        {
            //Destroy(item.Icon);
            item.InInventory = false;
            gameManager.Player.InventoryController.RemoveItem(item.Data);
        }
    }

    public void Equip(InventoryItem invItem)
    {
        var ID = invItem.Data.ID;

        Item item = Array.Find(items, e => e.ID == ID);

        if (item == null || item.Equipped)
            return;


        if (equippedItemID != "")
            RemoveItem(equippedItemID);

        GameObject go = Instantiate(iconPrefab, iconParent);
        go.GetComponent<Image>().sprite = item.Sprite;
        go.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
        item.Icon = go;

        item.Equipped = true;

        equippedItemID = item.ID;
    }

    public void Unequip(InventoryItem invItem)
    {
        var ID = invItem.Data.ID;

        Item item = Array.Find(items, e => e.ID == ID);

        if (item == null || !item.Equipped)
            return;


        Destroy(item.Icon);
        item.Equipped = false;

        equippedItemID = "";
    }

    public void RemoveItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.InInventory)
            return;


        Destroy(item.Icon);
        item.Equipped = false;

        equippedItemID = "";
    }

    public void EquipItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || item.InInventory)
            return;

        item.InInventory = true;

        gameManager.Player.InventoryController.AddItem(item.Data);

        ShowNotification(item.Name);

        EffectsManager.Instance.audioManager.Play("Item");
    }

    public void TrueRemoveItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.InInventory)
            return;

        //Destroy(item.Icon);
        item.InInventory = false;

        gameManager.Player.InventoryController.RemoveItem(item.Data);
    }
}

[System.Serializable]
public class Item
{
    [HideInInspector] public string ID;
    [HideInInspector] public string Name;
    [HideInInspector] public Sprite Sprite;
    [HideInInspector] public bool LimitedUses;
    [HideInInspector] public int Uses;
    [HideInInspector] public bool Equipped;
    [HideInInspector] public bool InInventory;
    [HideInInspector] public GameObject Icon;
    public ItemData Data;

    public Item(ItemData data)
    {
        Data = data;
        ID = data.ID;
        Name = data.Name;
        Sprite = data.Sprite;
        LimitedUses = data.LimitedUses;
        Uses = data.Uses;
    }
}
