using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
public class EventDependencyHunter : MonoBehaviour
{
    private static List<EventReferenceInfo> dependencies;

    public static List<EventReferenceInfo> FindDependencies(string methodName)
    {
        var depens = FindAllUnityEventsReferences();
        var onlyWithName = new List<EventReferenceInfo>();

        foreach (var d in depens)
        {
            if (d.MethodNames.Where(m => m.ToLower().Contains(methodName.ToLower())).Count() > 0)
            {
                var indexes = d.MethodNames.Where(n => n.ToLower().Contains(methodName.ToLower())).Select(n => d.MethodNames.IndexOf(n)).ToArray();

                var info = new EventReferenceInfo();
                info.Owner = d.Owner;
                info.Event = d.Event;

                foreach (var i in indexes)
                {
                    info.Listeners.Add(d.Listeners[i]);
                    info.MethodNames.Add(d.MethodNames[i]);
                }

                onlyWithName.Add(info);
            }
        }

        return onlyWithName.Count > 0 ? onlyWithName : depens;
    }

    public static List<EventReferenceInfo> FindDependencies()
    {
        return FindAllUnityEventsReferences();
    }

    public static List<EventReferenceInfo> FindAllUnityEventsReferences()
    {
        var behaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        var events = new Dictionary<MonoBehaviour, List<UnityEventBase>>();

        foreach (var b in behaviours)
        {
            var info = b.GetType().GetTypeInfo();
            var evnts = info.DeclaredFields.Where(f => f.FieldType.IsSubclassOf(typeof(UnityEventBase))).ToList();
            foreach (var e in evnts)
            {
                var unityEvent = e.GetValue(b) as UnityEventBase;

                if (!events.ContainsKey(b))
                {
                    events[b] = new List<UnityEventBase>();
                }

                events[b].Add(unityEvent);
            }
        }

        var infos = new List<EventReferenceInfo>();

        foreach (var e in events)
        {
            foreach (var unityEvent in e.Value)
            {
                int count = unityEvent.GetPersistentEventCount();
                var info = new EventReferenceInfo();
                info.Owner = e.Key;
                info.Event = unityEvent;

                for (int i = 0; i < count; i++)
                {
                    var obj = unityEvent.GetPersistentTarget(i);
                    var method = unityEvent.GetPersistentMethodName(i);

                    info.Listeners.Add(obj as MonoBehaviour);
                    info.MethodNames.Add(obj.GetType().Name.ToString() + "." + method);
                }

                infos.Add(info);
            }
        }

        return infos;
    }

    public static IEnumerable<SerializedProperty> GetPropertiesWithAttribute<TAttribute>(SerializedObject serializedObject)
    {
        var targetObjectType = serializedObject.targetObject.GetType();
        var property = serializedObject.GetIterator();

        while (property.Next(true))
        {
            var field = targetObjectType.GetField(property.name);

            Debug.Log(field);

            if (field != null && Attribute.IsDefined(field, typeof(TAttribute)))
            {
                Debug.Log(property);

                yield return property.Copy();
            }
        }
    }

}

public class EventReferenceInfo
{
    public MonoBehaviour Owner { get; set; }
    public UnityEventBase Event { get; set; }
    public List<MonoBehaviour> Listeners { get; set; } = new List<MonoBehaviour>();
    public List<string> MethodNames { get; set; } = new List<string>();
}
#endif