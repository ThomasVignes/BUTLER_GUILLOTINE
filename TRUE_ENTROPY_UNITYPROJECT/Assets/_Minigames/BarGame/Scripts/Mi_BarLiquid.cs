using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mi_BarLiquid
{
    public Mi_BarLiquidType Type;
    public float Amount;

    public Mi_BarLiquid(Mi_BarLiquidType type, float amount) { Type = type; Amount = amount; }
}
