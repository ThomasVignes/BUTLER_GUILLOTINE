using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSecretEnd : MonoBehaviour
{
    [SerializeField] Transform jerTransform;

    public void DisableFog()
    {
        jerTransform.localPosition = new Vector3(jerTransform.localPosition.x, -2.09f, jerTransform.localPosition.z);

        RenderSettings.fog = false;
    }

    public void NextScene()
    {
        StartCoroutine(C_NextScene());
    }

    IEnumerator C_NextScene()
    {

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
