using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ListToPopupAttribute : PropertyAttribute
{
    public Type Type;
    public string propertyName;
    public ListToPopupAttribute(Type m_Type, string m_propertyName)
    {
        Type = m_Type;
        propertyName = m_propertyName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListToPopupAttribute))]
public class ListToPopupDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ListToPopupAttribute atb = attribute as ListToPopupAttribute;
        List<string> strList = null;

        if (atb.Type.GetField(atb.propertyName) != null)
        {
            strList = atb.Type.GetField(atb.propertyName).GetValue(atb.Type) as List<string>;
        }

        if (strList != null && strList.Count != 0)
        {
            int selectedIndex = Mathf.Max(strList.IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, strList.ToArray());
            property.stringValue = strList[selectedIndex];
        }
        else
            EditorGUI.PropertyField(position, property, label);
    }
}
#endif
