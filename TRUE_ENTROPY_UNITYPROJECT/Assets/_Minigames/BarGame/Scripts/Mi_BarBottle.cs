using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarBottle : Mi_BarItem
{
    [SerializeField] private Mi_BarLiquidType Type;
    [SerializeField] private float TurnSpeed;
    [SerializeField] private float MaxRotation;
    [SerializeField] private float DropRate;
    [SerializeField] private GameObject Droplet;
    [SerializeField] private Transform Nozzle;
    
    public Mi_BarRecipient TargetRecipient;

    private Mi_BarClickDrag clickDrag;
    private float DropTimer = 0;

    private void Start()
    {
        clickDrag = GetComponent<Mi_BarClickDrag>();
    }

    void Update()
    { 
        if (clickDrag.Dragging)
        {
            if (TargetRecipient != null)
            {
                var orientation = Mathf.Sign(transform.position.x - TargetRecipient.transform.position.x);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, MaxRotation * orientation), TurnSpeed);

                if (Time.time > DropTimer)
                {
                    DropTimer += DropRate;
                    float randX = Random.Range(-2f, 2f);
                    var drop = Instantiate(Droplet, new Vector3(Nozzle.position.x + randX/100, Nozzle.position.y, Nozzle.position.z), Quaternion.identity);
                    drop.GetComponent<Mi_BarDroplet>().Type = Type;
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), TurnSpeed);
            }
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }


}
