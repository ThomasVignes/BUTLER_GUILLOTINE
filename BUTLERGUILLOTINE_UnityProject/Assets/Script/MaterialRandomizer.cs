using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] SkinnedMeshRenderer[] skinnedMeshRenderers;
    [SerializeField] MeshRenderer[] meshRenderers;
    [SerializeField] GameObject active1, active2;

    private void Start()
    {
        foreach (var renderer in skinnedMeshRenderers)
        {
            renderer.material = GetMat();
        }

        foreach (var renderer in meshRenderers)
        {
            renderer.material = GetMat();
        }

        int rand = Random.Range(0, 2);
        active1.SetActive(rand == 0);
        active2.SetActive(rand == 1);
    }

    Material GetMat()
    {
        int rand = Random.Range(0, materials.Length);

        return materials[rand];
    }
}
