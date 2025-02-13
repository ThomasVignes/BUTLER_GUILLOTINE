using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WIPWarning : MonoBehaviour
{
    [SerializeField] float delayBeforeWarning;
    [SerializeField] GameObject canvas;

    private void Awake()
    {
        StartCoroutine(C_Warn());
    }

    IEnumerator C_Warn()
    {
        yield return new WaitForSeconds(delayBeforeWarning);

        Time.timeScale = 0;
        canvas.SetActive(true);
    }


    public void Resume()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
