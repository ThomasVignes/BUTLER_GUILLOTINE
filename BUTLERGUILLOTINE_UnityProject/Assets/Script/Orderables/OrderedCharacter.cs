using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderedCharacter : MonoBehaviour
{
    [Header("Parameters")]
    public OrderedAction role;
    [SerializeField] protected bool runs;
    [SerializeField] protected float appearDuration;
    [SerializeField] protected float timeBeforeAction;
    [SerializeField] protected float timeBeforeFadeout, timeBeforeDeactivated;
    [SerializeField] protected float actionDistance;
    [SerializeField] protected float spawnDistanceFromTarget;

    [Header("References")]
    [SerializeField] protected Character character;
    [SerializeField] protected Ghost ghostParams;
    protected Transform ruth;

    protected bool active, appearing, moving, action;

    protected Transform actionTarget;
    protected Vector3 targetPos;

    public void Init(Transform ruth)
    {
        character.Init();
        character.gameObject.SetActive(false);

        this.ruth = ruth;
    }

    public virtual void Step()
    {
        if (character.gameObject.activeSelf)
            character.Step();

        if (moving)
        {
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

    public void ConstantStep()
    {
        if (character.gameObject.activeSelf)
            character.ConstantStep();
    }

    public void Summon(Vector3 apparitionSpot, Transform target)
    {
        if (active)
            return;

        active = true;

        actionTarget = target;
        targetPos = target.position;

        Vector3 dir = Vector3.Normalize(targetPos - ruth.position);

        character.transform.position = ruth.position + dir * spawnDistanceFromTarget;
        character.transform.LookAt(targetPos, Vector3.up);
        character.gameObject.SetActive(true);


        StartCoroutine(C_SummonTo(target.position));
    }

    public void SummonNoTarget(Vector3 apparitionSpot, Vector3 targetSpot)
    {
        if (active)
            return;

        active = true;


        targetPos = targetSpot;

        Vector3 dir = Vector3.Normalize(targetPos - ruth.position);

        character.transform.position = ruth.position + dir * spawnDistanceFromTarget;
        character.gameObject.SetActive(true);

        StartCoroutine(C_SummonTo(targetSpot));
    }

    protected IEnumerator C_SummonTo(Vector3 target)
    {
        appearing = true;
        ghostParams.ManualAppear();

        yield return new WaitForSeconds(appearDuration);

        appearing = false;

        character.SetDestination(target);
        character.ToggleRun(runs);


        //Security
        yield return new WaitForSeconds(0.2f);

        moving = true;
    }

    public virtual void TryAction()
    {
        if (actionTarget == null)
        {
            Vanish();
        }
        else
        {
            Action();
        }
    }

    [ContextMenu("Action")]
    public virtual void Action()
    {
        StartCoroutine(C_Action());
    }

    protected virtual IEnumerator C_Action()
    {
        action = true;

        yield return new WaitForSeconds(timeBeforeAction);

        action = false;

        yield return new WaitForSeconds(timeBeforeFadeout);

        Vanish();
    }

    public void Vanish()
    {
        appearing = false;
        moving = false;
        action = false;
        actionTarget = null;
        StartCoroutine(C_Vanish());
    }

    IEnumerator C_Vanish()
    {
        yield return new WaitForSeconds(timeBeforeFadeout);

        ghostParams.ManualVanish();

        yield return new WaitForSeconds(timeBeforeDeactivated);

        active = false;
        character.gameObject.SetActive(false);
    }
}

public enum OrderedAction
{
    Photographer,
    Fighter
}
