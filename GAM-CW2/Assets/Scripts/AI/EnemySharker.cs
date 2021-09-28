using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySharker : EnemyAI
{
    public override void Start()
    {
        base.Start();

        DefaultBehaviour = new State_Circle(this);
        playerSeen = true;
    }

}
