using UnityEngine;
using System.Collections;

public class Mi_BarClickDrag : MonoBehaviour
{
    public bool Dragging;
    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        Dragging = true;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        if (gameObject.GetComponent<Rigidbody2D>() != null)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }

    private void OnMouseUp()
    {
        Dragging = false;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
        if (gameObject.GetComponent<Rigidbody2D>() != null)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
    }

    private void OnMouseOver()
    {
        if (Dragging)
            Mi_BarGameMaster.Instance.CursorManager.ChangeCursorTo("Closed");
    }

    private void OnMouseExit()
    {
        Mi_BarGameMaster.Instance.CursorManager.ChangeCursorTo("Open");
    }

}