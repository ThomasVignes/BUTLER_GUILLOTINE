using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mi_BarLiquidType
{
    Jack,
    Vodka,
    OrangeJuice
}

public enum Mi_BarMixType
{
    None,
    JV
}

public class Mi_BarRecipeManager : MonoBehaviour
{
    public static Mi_BarRecipeManager Instance;

    [Header("Values")]
    [SerializeField] private float detectionLimit;

    [Header("Setup")]
    
    [SerializeField] private List<Mi_BarRecipe> recipes = new List<Mi_BarRecipe>();

    [Header("Colors")]
    [SerializeField] private List<Mi_BarLiquidColor> liquidColors = new List<Mi_BarLiquidColor>();
    [SerializeField] private List<Mi_BarMixColor> mixColors = new List<Mi_BarMixColor>();

    private void Awake()
    {
        Instance = this;

        foreach (Mi_BarRecipe r in recipes)
        {
            r.Ingredients.Sort();
        }
    }

    public Mi_BarMixType CheckForRecipe(List<Mi_BarLiquid> content)
    {
        List<Mi_BarLiquidType> mix = new List<Mi_BarLiquidType>();

        foreach (Mi_BarLiquid l in content)
        {
            if (l.Amount > detectionLimit)
                mix.Add(l.Type);
        }

        mix.Sort();

        foreach (Mi_BarRecipe r in recipes)
        {
            if (CheckRecipeMatch(mix, r.Ingredients))
                return r.Result;
        }

        return Mi_BarMixType.None;
    }

    public Color CheckForColor(Mi_BarLiquidType Type)
    {
        foreach (Mi_BarLiquidColor col in liquidColors)
        {
            if (col.Type == Type)
                return col.Color;
        }

        Debug.LogError("Color not set");
        return Color.white;
    }

    public Color CheckForColor(Mi_BarMixType Type)
    {
        foreach (Mi_BarMixColor col in mixColors)
        {
            if (col.Type == Type)
                return col.Color;
        }

        Debug.LogError("Color not set");
        return Color.white;
    }

    bool CheckRecipeMatch(List<Mi_BarLiquidType> mix, List<Mi_BarLiquidType> recipe)
    {
        if (mix.Count != recipe.Count)
            return false;
        for (int i = 0; i < mix.Count; i++)
        {
            if (mix[i] != recipe[i])
                return false;
        }
        return true;
    }
}
