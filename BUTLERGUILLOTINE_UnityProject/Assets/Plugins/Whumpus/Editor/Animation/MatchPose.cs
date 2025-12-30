using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Whumpus.Editor
{
    public class MatchPose : EditorWindow
    {
        public int Iterations = 10;
        public GameObject TargetRoot, RefRoot;
        public bool MatchPosition = true, MatchRotation = true, MatchScale = false;
        Vector2 scrollPos;
        Vector3 offset;
        bool calculating;

        [MenuItem("Whumpus/Animation/MatchPose")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MatchPose));
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height/2));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TargetRoot = (GameObject)EditorGUILayout.ObjectField("Target root", TargetRoot, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            RefRoot = (GameObject)EditorGUILayout.ObjectField("Ref root", RefRoot, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            MatchPosition = EditorGUILayout.Toggle("Match ref position", MatchPosition);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            MatchRotation = EditorGUILayout.Toggle("Match ref rotation", MatchRotation);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            MatchScale = EditorGUILayout.Toggle("Match ref scale", MatchScale);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            Iterations = EditorGUILayout.IntField(Iterations);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Match") && !calculating)
            {
                calculating = true;

                for (int i = 0; i < Iterations; i++)
                    Match(TargetRoot, RefRoot, true);

                calculating = false;
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndScrollView();
        }

        void Match(GameObject Target, GameObject Ref, bool First)
        {
            Vector3 offset = Target.transform.position - Ref.transform.position;

            if (First) 
                Target.transform.position = Ref.transform.position;

            foreach (Transform child in Target.transform)
            {
                Match(child.gameObject, Ref, false);

                var clone = GetCloneOfLimb(Ref, child.gameObject);

                if (clone == null)
                    continue;

                if (child.name == clone.name)
                {
                    if (MatchPosition)
                        child.position = clone.transform.position;
                    if (MatchRotation)
                        child.rotation = clone.transform.rotation;
                    if (MatchScale)
                        child.localScale = clone.transform.localScale;
                }
            }

            if (First)
            {
                Target.transform.position = Ref.transform.position + offset;
                Target.transform.rotation = Ref.transform.rotation;
            }
        }

        GameObject GetCloneOfLimb(GameObject RefRoot, GameObject Limb)
        {
            var transform = RecursiveFindChild(RefRoot.transform, Limb.name);

            if (transform == null)
                return null;
            else
                return transform.gameObject;
        }

        Transform RecursiveFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }
}