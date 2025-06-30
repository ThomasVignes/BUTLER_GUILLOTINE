using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMovement : MonoBehaviour
{
    [SerializeField] float timerMultiplier;
    [SerializeField] int startSeconds, startMinutes, startHours;
    [SerializeField] Transform seconds, minutes, hours;
    [SerializeField] AudioSource tick;

    float sAngle, mAngle, hAngle;
    float timer, mult;

    int secs, lastSecs, mins, hrs;

    Quaternion sX, mX, hX;

    private void Awake()
    {
        sAngle = 360 / 60;
        mAngle = 360 / 60;
        hAngle = 360 / 24;

        sX = seconds.rotation;
        mX = minutes.rotation;
        hX = hours.rotation;

        ResetClock();

        mult = 1;
    }

    public void ResetClock()
    {
        timer = startSeconds;
        seconds.Rotate(-transform.up, sAngle * secs);

        mins = startMinutes;
        minutes.Rotate(-transform.up, mAngle * mins);

        hrs = startHours;
        hours.Rotate(-transform.up, hAngle * hrs);

        secs = 0;
        lastSecs = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime * mult;

        secs = Mathf.RoundToInt(timer % 60);

        if (secs - lastSecs >= 1)
        {
            seconds.Rotate(-transform.up, sAngle);

            lastSecs = secs;

            mult = Random.Range(1, timerMultiplier);

            tick.Play();
        }

        if (secs >= 60)
        {
            secs = 0;
            lastSecs = 0;
            timer = 0;

            minutes.Rotate(-transform.up, mAngle);
            seconds.rotation = sX;

            mins++;
        }

        if (mins > 60) 
        {
            mins = 0;

            hours.Rotate(-transform.up, hAngle);
            minutes.rotation = mX;

            hrs++;
        }

        if (hrs > 24)
        {
            hours.rotation = hX;

            hrs = 0;
        }
    }
}
