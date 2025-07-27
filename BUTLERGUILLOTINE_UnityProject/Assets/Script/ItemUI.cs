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
    [SerializeField] Color disabledTextColor;

    InventoryController controller;

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
    }
}
