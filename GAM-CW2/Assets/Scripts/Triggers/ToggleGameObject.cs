using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObject : Trigger
{
    public bool SetBoolean = false;
    public GameObject[] Objects;

    public override void TriggerAction()
    {
        //Toggle all gameobjects
        foreach (GameObject obj in Objects)
        {
            //obj.SetActive(!obj.activeSelf);
            obj.SetActive(SetBoolean);
        }
    }

}
