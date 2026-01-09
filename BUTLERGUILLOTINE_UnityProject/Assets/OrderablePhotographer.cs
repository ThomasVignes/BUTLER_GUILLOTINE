using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderablePhotographer : OrderedCharacter
{
    public override void TryAction()
    {
        if (actionTarget != null && actionTarget.GetComponent<Portal>())
        {
            Action();
        }
        else
        {
            Vanish();
        }
    }

    protected override IEnumerator C_Action()
    {
        action = true;

        yield return new WaitForSeconds(timeBeforeAction);

        actionTarget.GetComponent<Portal>().Toggle(true);
        actionTarget = null;

        action = false;

        yield return new WaitForSeconds(timeBeforeFadeout);

        Vanish();
    }
}
