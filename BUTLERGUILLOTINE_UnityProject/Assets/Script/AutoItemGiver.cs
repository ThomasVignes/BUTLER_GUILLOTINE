using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoItemGiver : MonoBehaviour
{
    [SerializeField] string[] items;

    private void Start()
    {
        StartCoroutine(C_Delay());
    }

    IEnumerator C_Delay()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var item in items)
        {
            GameManager.Instance.InventoryManager.EquipItem(item, false);
        }
    }
}
