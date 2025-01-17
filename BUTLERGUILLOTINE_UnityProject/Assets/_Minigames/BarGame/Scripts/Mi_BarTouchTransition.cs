using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarTouchTransition : MonoBehaviour
{
    [SerializeField] private Mi_BarTransitionMaster transitionMaster;
    private Vector2 initialPosition;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                initialPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // get the moved direction compared to the initial touch position
                var direction = touch.position - initialPosition;

                // get the signed x direction
                // if(direction.x >= 0) 1 else -1
                var signedDirection = Mathf.Sign(direction.x);
                var POWER = Mathf.Abs(direction.magnitude);

                if (transitionMaster.Left && signedDirection == 1 && POWER > 0.5f)
                    transitionMaster.UpdateCam();
                
                if (!transitionMaster.Left && signedDirection == -1 && POWER > 0.5f)
                    transitionMaster.UpdateCam();
            }

        }
    }
}
