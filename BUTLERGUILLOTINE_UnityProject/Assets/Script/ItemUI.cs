using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [Header("Values")]
    public string EquipText;
    public string UnequipText;

    [Header("References")]
    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] Button equipButton;
    [SerializeField] TextMeshProUGUI equipText;
    [SerializeField] TextMeshProUGUI examineText;
    [SerializeField] Color disabledTextColor;

    InventoryController controller;

    bool corrupted;
    string corruptedText;

    Color originalColor;

    public void Init(InventoryController controller)
    {
        this.controller = controller;
        content.SetActive(false);

        originalColor = equipText.color;

        equipText.text = EquipText;
    }

    public void UpdateName(string name)
    {
        if (corrupted)
            name = Corrupt(name);

        itemName.text = name;
    }

    public void Show(bool show)
    {
        content.SetActive(show);
    }

    public void EquipItem()
    {
        EffectsManager.Instance.audioManager.Play("SmallClick");

        controller.EquipItem();
    }

    public void UpdateEquipText(bool equipped)
    {
        if (equipped)
            equipText.text = UnequipText;
        else
            equipText.text = EquipText;
    }

    public void ExamineItem()
    {
        EffectsManager.Instance.audioManager.Play("SmallClick");
        controller.ExamineSelected();
    }

    public void ToggleEquippable(bool equippable)
    {
        equipButton.interactable = equippable;

        if (!equippable)
            equipText.color = disabledTextColor;
        else
            equipText.color = originalColor;
    }


    public void CorruptText(string corruptedText)
    {
        corrupted = true;
        this.corruptedText = corruptedText;

        UnequipText = Corrupt(UnequipText);
        EquipText = Corrupt(EquipText);
        examineText.text = Corrupt(examineText.text);
    }

    string Corrupt(string text)
    {
        string toCorrupt = text;

        int count = toCorrupt.Length;
        toCorrupt = "";

        for (int i = 0; i < count; i++)
        {
            toCorrupt += corruptedText;
        }

        return toCorrupt;
    }
}
