using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whumpus
{
    public static class WhumpusUtilities 
    {
        public static int ToLayer(int bitmask)
        {
            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask = bitmask >> 1;
                result++;
            }
            return result;
        }

        public static void ResetAllAnimatorTriggers(this Animator animator)
        {
            foreach (var trigger in animator.parameters)
            {
                if (trigger.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(trigger.name);
                }
            }
        }

        public static bool IsInScreen(Transform transform, float lenience)
        {
            var cam = Camera.main;

            Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

            float maxThrehold = 1 + lenience;
            float minThreshold = 0 - lenience;

            if (viewPos.x <= maxThrehold &&  viewPos.y <= maxThrehold)
                if (viewPos.x >= minThreshold && viewPos.y >= minThreshold)
                {
                    return true;
                }

            return false;
        }
    }
}
