using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarMenuPage : MonoBehaviour
{
    public bool Shown;
    public bool Semi;
    public Mi_BarMenu Menu;
    [SerializeField] private GameObject Button;

    private void Update()
    {
        if (Button != null)
            Button.SetActive(Shown);

        if (Shown)
        {
            if (transform.position != Menu.Shown.position)
            {
                transform.position = Vector3.Lerp(transform.position, Menu.Shown.position, 0.4f);
            }
        }
        else
        {
            if (Semi)
            {
                if (transform.position != Menu.Half.position && !Shown)
                {
                    transform.position = Vector3.Lerp(transform.position, Menu.Half.position, 0.2f);
                }
            }
            else
            {
                if (transform.position != Menu.Hidden.position && !Shown)
                {
                    transform.position = Vector3.Lerp(transform.position, Menu.Hidden.position, 0.2f);
                }
            }
        }
    }
}
