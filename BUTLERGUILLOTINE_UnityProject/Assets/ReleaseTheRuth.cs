using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReleaseTheRuth : MonoBehaviour
{
    [SerializeField] float delayBeforeTeleport;
    [SerializeField] Character ruth;
    [SerializeField] Transform[] warpPoints;

    public UnityEvent OnCatch;

    Transform targ;

    bool chasing;

    float timer;
    bool cd;

    void Update()
    {
        if (chasing)
        {
            
            ruth.SetDestination(targ.position, true);
            return;
        }

        if (cd)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
                Release();
        }
    }

    public void Countdown()
    {
        cd = true;
        timer = delayBeforeTeleport;
    }

    public void Release()
    {
        chasing = true;

        targ = GameManager.Instance.PlayerFollower.transform;

        Vector3 furthest = targ.position;

        foreach (var point in warpPoints)
        {
            if (Vector3.Distance(point.position, targ.position) > Vector3.Distance(furthest, targ.position))
                furthest = point.position;
        }

        ruth.TeleportCharacter(furthest, false);
        ruth.ToggleRun(true);
    }

    public void Caught()
    {
        GameManager.Instance.CinematicManager.PlayCinematic("Chase", true);

        OnCatch?.Invoke();
    }
}
