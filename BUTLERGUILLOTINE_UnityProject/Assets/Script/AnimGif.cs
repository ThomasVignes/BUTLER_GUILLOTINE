using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimGif : MonoBehaviour
{
    public bool OccasoftwareTriplanarMode;
    [SerializeField] private Texture2D[] frames;
    [SerializeField] private float fps = 10.0f;
    [SerializeField] Renderer[] additionalInputs;
    private Material mat;

    void Start()
    {
        var renderer = GetComponent<Renderer>();

        if (renderer != null)
            mat = renderer.material;
    }

    void Update()
    {
        int index = (int)(Time.time * fps);
        index = index % frames.Length;

        var texture = frames[index];

        if (mat != null)
            UpdateTexture(mat, texture);

        foreach (var item in additionalInputs)
        {
            UpdateTexture(item.material, texture);
        }
    }

    void UpdateTexture(Material mat, Texture2D texture)
    {
        if (OccasoftwareTriplanarMode)
        {
            mat.SetTexture("Texture2D_61ff577471574ea7b1ed02152eb4b594", texture);
            mat.SetTexture("_Albedo_Map_Y", texture);
            mat.SetTexture("_Albedo_Map_Z", texture);
        }
        else
            mat.mainTexture = texture;
    }
}
