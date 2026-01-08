using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class OrderableFighter : OrderedCharacter
{
    [Header("Fighter Settings")]
    [SerializeField] float grabDelay;
    [SerializeField] float grabPause;
    [SerializeField] float aimTime;
    [SerializeField] float throwForce;
    [SerializeField] Animator animator;
    [SerializeField] GameObject ragdoll;
    [SerializeField] ConfigurableJoint grabJoint;
    [SerializeField] Transform spawnPoint;

    bool aiming;

    public override void Step()
    {
        //Might need to use the enemy AI instead
        if (character.gameObject.activeSelf)
            character.Step();

        if (moving)
        {
            if (actionTarget != null)
            {
                character.SetDestination(actionTarget);
                targetPos = actionTarget.position;
            }

            float distance = Vector3.Distance(character.transform.position, targetPos);

            if (distance < actionDistance)
            {
                moving = false;
                character.Pause();
                TryAction();
            }

            if (!character.Moving && !character.Rotating)
            {
                //Vanish();
            }
        }
    }
    public override void TryAction()
    {
        Action();
    }

    public override void Action()
    {
        StartCoroutine(C_Action());
    }

    protected override IEnumerator C_Action()
    {
        action = true;

        animator.SetTrigger("Grab");

        yield return new WaitForSeconds(grabDelay);

        if (actionTarget != null)
            actionTarget.GetComponent<Lifeform>().DeathNoRagdoll();

        GameObject go = Instantiate(ragdoll);

        go.transform.position = grabJoint.transform.position;
        go.transform.rotation = grabJoint.transform.rotation;

        /*
        go.GetComponent<InitRagdoll>().torsoRb.transform.position = grabJoint.transform.position;
        go.GetComponent<InitRagdoll>().torsoRb.transform.rotation = grabJoint.transform.rotation;
        */

        grabJoint.connectedBody = go.GetComponent<InitRagdoll>().torsoRb;

        yield return new WaitForSeconds(grabPause);

        aiming = true;
        animator.SetTrigger("Windup");

        yield return new WaitForSeconds(aimTime);

        aiming = false;

        animator.SetTrigger("Throw");

        grabJoint.connectedBody = null;

        go.GetComponent<InitRagdoll>().torsoRb.AddForce(character.transform.forward.normalized * throwForce);

        action = false;

        yield return new WaitForSeconds(timeBeforeFadeout);


        Vanish();
    }
}
