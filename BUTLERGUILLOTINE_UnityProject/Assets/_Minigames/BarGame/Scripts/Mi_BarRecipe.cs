using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mi_BarRecipe 
{
    public Mi_BarMixType Result;
    public List<Mi_BarLiquidType> Ingredients = new List<Mi_BarLiquidType>();
}
