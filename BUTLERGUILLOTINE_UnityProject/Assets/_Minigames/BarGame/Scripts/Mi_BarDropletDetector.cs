using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarDropletDetector : MonoBehaviour
{
    [SerializeField] private Mi_BarRecipient Recipient;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Mi_BarDroplet>())
        {
            Recipient.AddDroplet(collision.GetComponent<Mi_BarDroplet>());
        }
    }
}
