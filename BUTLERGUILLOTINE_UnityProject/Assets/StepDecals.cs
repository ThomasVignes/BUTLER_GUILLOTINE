using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class StepDecals : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float maxdistance = 10;
    [SerializeField] Vector3 offset = Vector3.zero;

    [Header("Optimization")]
    [SerializeField] int maxDecals;

    [Header("References")]
    [SerializeField] Transform dir;
    [SerializeField] GameObject[] stepDecals;
    [SerializeField] Transform[] feet;
    [SerializeField] LayerMask ground;

    List<GameObject> steps = new List<GameObject>();

    public void PlaceDecal()
    {
        float lowest = float.MaxValue;
        int index = 99;

        for (int i = 0; i < feet.Length; i++)
        {
            var item = feet[i];

            float Ypos = item.position.y;

            if (Ypos < lowest)
            {
                lowest = Ypos;
                index = i;
            }
        }

        var foot = feet[index];
        GameObject decal = Instantiate(stepDecals[index], transform);

        steps.Add(decal);   

        decal.transform.forward = dir.forward;

        RaycastHit hit;
        Ray ray = new Ray(foot.position + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, 10, ground))
            decal.transform.position = hit.point + offset;

        CheckOptimization();
    }

    void CheckOptimization()
    {
        if (steps.Count > maxDecals) 
        {
            GameObject todestroy = steps[0];

            steps.RemoveAt(0);

            Destroy(todestroy);
        }
    }
}
