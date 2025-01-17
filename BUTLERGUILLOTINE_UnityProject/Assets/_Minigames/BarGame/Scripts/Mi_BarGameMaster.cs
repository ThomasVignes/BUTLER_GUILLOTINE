using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarGameMaster : MonoBehaviour
{
    public static Mi_BarGameMaster Instance;
    public Mi_BarCursorManager CursorManager;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        EffectsManager.Instance.audioManager.Play("MainTrack");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CallElevator();

    }

    public void ScreenShake()
    {
        impulseSource.GenerateImpulse();
    }

    public void CallElevator()
    {
        StartCoroutine(Elevator());
    }

    IEnumerator Elevator()
    {
        EffectsManager.Instance.audioManager.Play("ElevatorFRFRFR");
        var yes = 0.4f;
        for (int i = 0; i < 100; i++)
        {
            ScreenShake();
            yield return new WaitForSeconds(yes);
            yes = Mathf.Lerp(yes, 0.03f, 0.1f);
        }

        for (int i = 0; i < 55; i++)
        {
            ScreenShake();
            yield return new WaitForSeconds(0.1f);
        }

        for (int i = 0; i < 230; i++)
        {
            ScreenShake();
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < 100; i++)
        {
            ScreenShake();
            ScreenShake();
            ScreenShake();
            ScreenShake();
            yield return new WaitForSeconds(0.00000000001f);
        }

        yield return new WaitForSeconds(1.93f);

        EffectsManager.Instance.audioManager.Play("Elevator Ding");
    }

}
