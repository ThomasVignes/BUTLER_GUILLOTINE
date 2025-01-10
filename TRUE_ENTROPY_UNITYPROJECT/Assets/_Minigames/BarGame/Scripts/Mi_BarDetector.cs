using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarDetector : MonoBehaviour
{
    [SerializeField] private Mi_BarBottle Bottle;
    private List<Mi_BarRecipient> NearRecipients = new List<Mi_BarRecipient>();

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.parent.rotation.z * -1.0f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Mi_BarRecipient>() != null)
        {
            NearRecipients.Add(other.gameObject.GetComponent<Mi_BarRecipient>());
            Bottle.TargetRecipient = GetClosestRecipient();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Mi_BarRecipient>()!= null)
        {
            NearRecipients.Remove(other.gameObject.GetComponent<Mi_BarRecipient>());
            if (Bottle.TargetRecipient == other.gameObject.GetComponent<Mi_BarRecipient>())
            {
                Bottle.TargetRecipient = null;
            }
        }
    }

    Mi_BarRecipient GetClosestRecipient()
    {
        Mi_BarRecipient Min = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Mi_BarRecipient t in NearRecipients)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                Min = t;
                minDist = dist;
            }
        }
        return Min;
    }
}
