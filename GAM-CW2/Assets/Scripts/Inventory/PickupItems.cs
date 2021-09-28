using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItems : MonoBehaviour
{
    public Inventory inv;

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag.Equals("Item"))
        {

            IInventoryItem item = hit.gameObject.GetComponent<IInventoryItem>();
            if (item != null)
            {
                inv.addItem(item);
            }

        }
    }
 
}
