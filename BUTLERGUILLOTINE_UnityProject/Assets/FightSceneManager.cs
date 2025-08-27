using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneManager : MonoBehaviour
{
    [SerializeField] float delayBeforeNext;

    private void Start()
    {
        Cursor.visible = false;
    }

    public void NextScene()
    {
        StartCoroutine(C_NextScene());
    }

    IEnumerator C_NextScene()
    {
        yield return new WaitForSeconds(delayBeforeNext);

        Cursor.visible = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
