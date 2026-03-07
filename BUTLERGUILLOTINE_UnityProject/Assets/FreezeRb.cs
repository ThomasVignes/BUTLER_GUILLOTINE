using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRb : MonoBehaviour
{
    bool freezing;

    private void OnCollisionEnter(Collision collision)
    {
        if (freezing)
            return;

        if (collision.gameObject.GetComponent<Rigidbody>() == null)
        {
            freezing = true;
            StartCoroutine(C_Freeze());
        }
    }

    IEnumerator C_Freeze()
    {
        yield return new WaitForSeconds(0.3f);

        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<BoxCollider>());
        Destroy(this);
    }
}
