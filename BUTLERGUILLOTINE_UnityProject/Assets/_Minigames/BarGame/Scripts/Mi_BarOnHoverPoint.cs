using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarOnHoverPoint : MonoBehaviour
{
    private void OnMouseOver()
    {
        Mi_BarGameMaster.Instance.CursorManager.ChangeCursorTo("Point");
    }

    private void OnMouseExit()
    {
        Mi_BarGameMaster.Instance.CursorManager.ChangeCursorTo("Open");
    }
}
