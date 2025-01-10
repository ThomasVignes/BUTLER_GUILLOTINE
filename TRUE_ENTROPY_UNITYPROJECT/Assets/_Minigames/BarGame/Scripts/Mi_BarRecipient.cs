using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Mi_BarRecipient : Mi_BarItem
{
    [Header("Attributes")]
    public Mi_BarMixType currentMix;
    public float CurrentLiquid;
    public float MaxLiquid;
    public List<Mi_BarLiquid> Liquids = new List<Mi_BarLiquid>();

    public virtual void AddDroplet(Mi_BarDroplet droplet)
    {
        bool contains = false;
        int index = 0;

        for (int i = 0; i < Liquids.Count; i++)
        {
            if (Liquids[i].Type == droplet.Type)
            {
                contains = true;
                index = i;
            }
        }

        if (CurrentLiquid < MaxLiquid)
        {
            CurrentLiquid += droplet.Liquid;

            if (contains)
                Liquids[index].Amount += droplet.Liquid;
            else
                Liquids.Add(new Mi_BarLiquid(droplet.Type, droplet.Liquid));
        }
        else
            CurrentLiquid = MaxLiquid;

        Destroy(droplet.gameObject);
    }

}
