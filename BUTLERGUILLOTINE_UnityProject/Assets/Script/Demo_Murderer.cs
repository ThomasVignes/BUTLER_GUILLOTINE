using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Murderer : MonoBehaviour
{
    [SerializeField] string murdererKeyID;

    private void Start()
    {
        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        yield return new WaitForSeconds(1f);

        var persistentData = PersistentData.Instance;

        if (persistentData != null && persistentData.DemoMode && persistentData.HasKey)
            GiveKey();
    }

    [ContextMenu("Give key")]
    void GiveKey()
    {
        GameManager.Instance.InventoryManager.EquipItem(murdererKeyID);
    }
}
