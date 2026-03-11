using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownSpot : MonoBehaviour
{
    [SerializeField] Transform position;

    public void Dropdown()
    {
        GameManager.Instance.Player.Dropdown(position.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Dropdown();
    }
}
