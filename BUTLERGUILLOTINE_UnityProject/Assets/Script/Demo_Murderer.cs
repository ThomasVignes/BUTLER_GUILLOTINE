using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Demo_Murderer : MonoBehaviour
{
    [SerializeField] string murdererKeyID;
    [SerializeField] GameObject blackScreen;

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

    public void BarTransition()
    {
        ButlerEngineUtilities.ClearAllInterScenes();
        GameManager.Instance.ThemeManager.OverrideAmbiance("Empty");
    }

    public void SecretScene()
    {
        StartCoroutine(C_SecretScene());
    }

    IEnumerator C_SecretScene()
    {
        blackScreen.SetActive(true);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("DEMO_SECRET");
    }
}
