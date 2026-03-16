using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugNavigator : MonoBehaviour
{
    public bool Active;
    [SerializeField] Transform disclaimer;

    private void Awake()
    {
        if (!Active)
            Destroy(gameObject);
        else
        {
            if (disclaimer != null)
            {
                disclaimer.SetParent(null);
                disclaimer = null;
            }

            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                SceneManager.LoadScene("ContentWarning");
        }

        if (Input.GetKeyDown(KeyCode.P))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (Input.GetKeyDown(KeyCode.O) && GameManager.Instance != null)
            GameManager.Instance.SpeedupAllPlayers();
    }
}
