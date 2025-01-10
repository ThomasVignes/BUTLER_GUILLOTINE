using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarTransitionMaster : MonoBehaviour
{
    public static Mi_BarTransitionMaster Instance;
    [Header("References")]
    [SerializeField] private CinemachineBrain Brain;
    [SerializeField] private GameObject Counter;
    [SerializeField] private GameObject Bar;
    [SerializeField] private Mi_BarClickableCollider CounterTrigger;
    [SerializeField] private Mi_BarClickableCollider BarTrigger;

    [HideInInspector]
    public bool Left;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        CounterTrigger.Triggered += () => UpdateCam();
        BarTrigger.Triggered += () => UpdateCam();
        Counter.SetActive(true);
        Bar.SetActive(false);
        Left = true;
    }
    
    public void UpdateCam()
    {
        if (!Brain.IsBlending)
        {
            Counter.SetActive(!Counter.activeInHierarchy);
            CounterTrigger.gameObject.SetActive(!Counter.activeInHierarchy);
            Bar.SetActive(!Bar.activeInHierarchy);
            BarTrigger.gameObject.SetActive(!Bar.activeInHierarchy);
            Left = Counter.activeInHierarchy;
        }
    }
}
