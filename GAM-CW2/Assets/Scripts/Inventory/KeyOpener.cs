using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOpener : MonoBehaviour
{
    public Inventory inv;
    public bool inRange = false;
    public GameObject key;

    void Start()
    {
        // register with the event handler
        inv.ItemUsed += Inventory_ItemUsed;
    }

    void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        // check if the correct item is in use
        if ((e.item as MonoBehaviour).gameObject == key)
        {
            // check if in range
            if (inRange)
            {
                gameObject.GetComponent<Door>().Open();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }
}
