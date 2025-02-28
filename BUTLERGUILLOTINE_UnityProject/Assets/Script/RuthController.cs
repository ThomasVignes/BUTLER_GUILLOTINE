using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Whumpus;
using static Cinemachine.CinemachineOrbitalTransposer;

public class RuthController : PlayerController
{
    [Header("Ruth Settings")]
    [SerializeField] LayerMask orderableLayer;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] float drawDelay;
    [SerializeField] float castTime;
    [SerializeField] private float clickdelay;
    [SerializeField] bool canMoveDuringCast;
    [SerializeField] float delayBeforeArmCross;
    [SerializeField] ParticleSystem snapFx;
    [SerializeField] Rig rig;
    [SerializeField] Transform aimTarg;

    
    LayerMask moveLayer;
    LayerMask interactLayer;
    
    LayerMask wallLayer;

    bool casting, recovery;
    float castTimer;
    float crossTimer;
    bool crossed;

    OrderedCharacter orderedCharacter;

    public override void Init()
    {
        base.Init();

        ignoreLayer = GameManager.Instance.IgnoreLayers;
        moveLayer = GameManager.Instance.MoveLayer;
        interactLayer = GameManager.Instance.InteractLayer;
        wallLayer = GameManager.Instance.WallLayer;
    }

    public override void Step()
    {
        base.Step();

        if (Mathf.Abs(agent.velocity.magnitude) <= 0)
            crossTimer += Time.deltaTime;
        else
            crossTimer = 0;

        if (Input.GetKeyDown(KeyCode.G))
            animator.SetTrigger("Clap");

        animator.SetBool("ArmsCrossed", crossTimer > delayBeforeArmCross);

        if (casting)
        {
            crossTimer = 0;

            rig.weight = Mathf.Lerp(rig.weight, 1, 4f * Time.deltaTime);

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer))
            {
                Vector3 targPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                aimTarg.position = targPos;

                CursorStuff(hit.transform);
            }
        }
        else
        {
            cursorType = CursorType.Aim;

            rig.weight = Mathf.Lerp(rig.weight, 0, 4f * Time.deltaTime);
        }


        if (recovery)
        {
            if (castTimer < Time.time)
            {
                recovery = false;
            }
        }
    }

    public override void Special(Vector3 spot, GameObject hitObject)
    {
        if (recovery)
            return;

        base.Special(spot, hitObject);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, orderableLayer))
        {
            OrderedCharacter ord = hit.transform.gameObject.GetComponent<OrderedCharacter>();

            if (ord != null)
            {
                if (!ord.Selected)
                    ord.Select(true);

                ord.Pause();

                if (orderedCharacter != null && orderedCharacter != ord)
                    orderedCharacter.Select(false);

                orderedCharacter = ord;
            }

            animator.SetTrigger("Snap");
            snapFx.Play();
        }
        else
        {
            if (orderedCharacter != null)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer))
                {
                    orderedCharacter.ToggleRun(true);
                    //orderedCharacter.SetDestination(hit.point);
                    TryOrder(orderedCharacter);

                    animator.SetTrigger("Clap");
                }
            }
        }

        castTimer = Time.time + castTime;
        recovery = true;

    }

    public override void ToggleSpecial(bool active)
    {
        casting = active;
        animator.SetBool("Aim", active);

        if (casting)
        {
            if (canMoveDuringCast)
                ToggleRun(false);

            castTimer = Time.time + drawDelay;
        }

        base.ToggleSpecial(active);

        if (!canMoveDuringCast)
        {
            computing = false;
            agent.isStopped = active;

            if (active)
            {
                PausePath();
            }
            else
            {
                agent.ResetPath();
            }
        }
    }


    private void TryOrder(Character ordered)
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer))
        {
            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
            {
                return;
            }

            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                ordered.SetDestination(interactable.GetTargetPosition(), interactable);

                if (interactable is PickupInteractable)
                {
                    ordered.PickUpAnim();
                }

                return;
            }

            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer))
            {
                ordered.SetDestination(hit.point);
                return;
            }
        }
    }

    private int clicked;
    private float clicktime;

    private void CursorStuff(Transform transform)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (orderedCharacter == null)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, orderableLayer))
                cursorType = CursorType.AimLook;
            else
                cursorType = CursorType.Aim;

            return;
        }


        if (transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
        {
            cursorType = CursorType.Aim;
            return;
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, orderableLayer))
        {
            cursorType = CursorType.AimLook;
            return;
        }

        Interactable interactable = transform.gameObject.GetComponent<Interactable>();

        if (interactable != null)
        {
            cursorType = CursorType.AimLook;
            return;
        }

        if (transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer))
        {
            cursorType = CursorType.AimMove;
            return;
        }
    }

}
