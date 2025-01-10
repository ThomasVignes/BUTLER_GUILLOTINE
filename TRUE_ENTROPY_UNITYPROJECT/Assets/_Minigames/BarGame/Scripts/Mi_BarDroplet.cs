using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarDroplet : MonoBehaviour
{
    public Mi_BarLiquidType Type;
    public float Liquid;
    [SerializeField] private SpriteRenderer spr;

    private void Start()
    {
        spr.color = Mi_BarRecipeManager.Instance.CheckForColor(Type);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
