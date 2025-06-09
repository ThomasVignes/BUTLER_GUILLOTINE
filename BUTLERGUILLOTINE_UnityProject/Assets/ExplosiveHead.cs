using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveHead : MonoBehaviour
{
    public bool AutoOnStart;
    [SerializeField] float speedModifier;
    [SerializeField] float startBlastSpeed, endBlastSpeed;
    [SerializeField] float freezeDelay;
    [SerializeField] GameObject[] headPieces;
    [SerializeField] Transform center;
    [SerializeField] GameObject[] hideThese;

    float blastSpeed;
    bool move;

    private void Start()
    {
        if (AutoOnStart)
            Blast();
    }

    private void Update()
    {
        if (!move)
            return;

        blastSpeed = Mathf.Lerp(blastSpeed, endBlastSpeed, speedModifier * Time.deltaTime);

        foreach (var item in headPieces)
        {
            Vector3 dir = item.transform.position - center.position;
            item.transform.position += dir.normalized * blastSpeed * Time.deltaTime;
        }
    }

    public void Blast()
    {
        /*
        foreach (var item in headPieces)
        {
            Rigidbody rb = item.AddComponent<Rigidbody>();

            rb.AddExplosionForce(blastSpeed, center.position, 1);
        }
        */

        move = true;
        blastSpeed = startBlastSpeed;

        foreach (var item in hideThese)
        {
            item.transform.localScale = Vector3.zero;
        }

        StartCoroutine(C_Freeze());
    }

    IEnumerator C_Freeze()
    {
        yield return new WaitForSeconds(freezeDelay);

        move = false;

        /*
        foreach (var item in headPieces)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();

            rb.isKinematic = true;
        }
        */
    }
}
