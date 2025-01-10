using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarCursorManager : MonoBehaviour
{
    [SerializeField] private List<Mi_BarCursorType> cursors = new List<Mi_BarCursorType>();

    public void ChangeCursorTo(string name)
    {
        foreach (var c in cursors)
        {
            if (c.Name == name)
                Cursor.SetCursor(c.Texture, Vector2.zero, CursorMode.Auto);
        }
    }
}
