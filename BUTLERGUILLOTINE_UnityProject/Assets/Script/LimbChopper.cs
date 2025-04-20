using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbChopper : MonoBehaviour
{
    public string ID;
    [SerializeField] GameObject previous, replacement;
    [SerializeField] Animator animator;
    [SerializeField] string chopTrigger;
    [SerializeField] GameObject additionalPrevious, additionalReplacement; 

    public void Chop()
    {
        previous.transform.localScale = Vector3.zero;
        replacement.SetActive(true);

        if (additionalPrevious != null) 
            additionalPrevious.transform.localScale = Vector3.zero;

        if (additionalReplacement != null)
            additionalReplacement.SetActive(true);

        if (animator != null )
            animator.SetTrigger(chopTrigger);
    }
}
