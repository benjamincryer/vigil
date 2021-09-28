using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollower : EnemyAI
{
    public override void Start()
    {
        base.Start();

        DefaultBehaviour = new State_Attack(this);
    }

}