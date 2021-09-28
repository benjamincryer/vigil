using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Trigger
{
    public override void TriggerAction()
    {
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");

        if (gc != null)
        {
            gc.GetComponent<GameManager>().SaveGame();
        }
    }
}
