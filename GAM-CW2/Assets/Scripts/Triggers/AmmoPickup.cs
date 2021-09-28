using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, ISaveable
{
    public Uuid id;
    public bool isTriggered = false;

    public Ammo AmmoType;
    public int PickupAmount = 10;

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

                //Re-enable renderers
                if(!isTriggered)
                {
                    Renderer[] rs = GetComponentsInChildren<Renderer>();
                    foreach (Renderer r in rs)
                    {
                        r.enabled = true;
                    }
                }

                break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.tag.Equals("Player"))
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src != null) src.Play();

            AmmoType.AddAmmo(PickupAmount);
            Destroy(gameObject, 1f);

            //Disable this script and all renderers until collection sound played
            Renderer[] rs = GetComponentsInChildren<Renderer>();
            foreach(Renderer r in rs)
            {
                r.enabled = false;
            }

            isTriggered = true;
        }
    }

}
