using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFlash : MonoBehaviour
{
    [SerializeField] float flash;
    [SerializeField] GameObject flashObject;

    public void Flash()
    {
        StartCoroutine(C_Flash());
    }

    IEnumerator C_Flash()
    {
        flashObject.SetActive(true);

        yield return new WaitForSeconds(flash);

        flashObject.SetActive(false);
    }
}
