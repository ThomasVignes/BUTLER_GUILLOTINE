using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootableLight : Lifeform
{
    public bool Lit;
    [SerializeField] Material baseMaterial, hitMaterial;
    [SerializeField] MeshRenderer lightMesh;

    public UnityEvent OnLit, OnUnlit;


    public override void Hurt(int damage)
    {
        if (Lit) return;

        ToggleLight(true);
    }
    
    public void ToggleLight(bool lit)
    {
        Lit = lit;

        if (lit)
        {
            lightMesh.material = hitMaterial;
            OnLit?.Invoke();
        }
        else
        {
            lightMesh.material = baseMaterial;
            OnUnlit?.Invoke();
        }
    }
}
