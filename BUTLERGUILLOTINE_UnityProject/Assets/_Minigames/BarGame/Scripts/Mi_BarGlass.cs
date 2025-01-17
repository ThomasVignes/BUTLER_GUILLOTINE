using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mi_BarGlass : Mi_BarRecipient
{
    [Header("References")]
    [SerializeField] private Image LiquidImage;


    private void Update()
    {
        LiquidImage.fillAmount = CurrentLiquid / MaxLiquid;
    }

    public override void AddDroplet(Mi_BarDroplet droplet)
    {
        base.AddDroplet(droplet);
        currentMix = Mi_BarRecipeManager.Instance.CheckForRecipe(Liquids);

        if (currentMix != Mi_BarMixType.None)
        {
            LiquidImage.color = Mi_BarRecipeManager.Instance.CheckForColor(currentMix);
        }
        else
            LiquidImage.color = Mi_BarRecipeManager.Instance.CheckForColor(droplet.Type);
    }
}
