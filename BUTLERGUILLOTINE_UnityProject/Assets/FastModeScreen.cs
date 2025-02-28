using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FastModeScreen : MonoBehaviour
{
    public void NextScene(bool fast)
    {
        PersistentData.Instance.FastMode = fast;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
