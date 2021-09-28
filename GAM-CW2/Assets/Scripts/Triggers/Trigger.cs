using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour, ISaveable
{
    public Uuid id;

    public bool TriggerOnCollision = true;
    public bool TriggerOnShoot = false;
    public bool isTriggered = false;
    public bool DestroyThis = true;

    public virtual void TriggerAction()
    {

    }

    public virtual void OnLoad()
    {
        TriggerAction();
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        SaveData.PickupData pickupData = new SaveData.PickupData();
        pickupData.id = id.uuid;
        pickupData.isTriggered = isTriggered;

        a_SaveData.m_PickupData.Add(pickupData);
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        foreach (SaveData.PickupData pickupData in a_SaveData.m_PickupData)
        {
            if (pickupData.id == id.uuid)
            {
                isTriggered = pickupData.isTriggered;

                if (isTriggered) OnLoad();//If triggered before save, do the trigger action

                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if ((other.tag.Equals("Player") && TriggerOnCollision) ||
            (other.tag.Equals("Projectile") && TriggerOnShoot))
        {
            TriggerAction();

            if (DestroyThis) isTriggered = true;
        }
    }
}
