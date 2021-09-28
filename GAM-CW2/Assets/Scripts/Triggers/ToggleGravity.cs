using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGravity : Trigger
{
    public bool SetBoolean = false;
    public Rigidbody r;

    public override void TriggerAction()
    {
        r.useGravity = SetBoolean;
    }
}
