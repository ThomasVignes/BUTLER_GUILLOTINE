using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarBell : MonoBehaviour
{
    Vector3 OriginalScale;

    private void Start()
    {
        OriginalScale = transform.localScale;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ring();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Mi_BarItem>() && collision.gameObject.GetComponent<Rigidbody2D>())
        {
            if (collision.relativeVelocity.magnitude > 8)
                Ring();
        }
    }

    private void Ring()
    {
        transform.DOKill();
        transform.localScale = OriginalScale;
        transform.DOPunchScale(new Vector3(0.04f, 0.04f, 0.04f), 0.5f, 10, 1);
        EffectsManager.Instance.audioManager.Play("Ding");
    }
}
