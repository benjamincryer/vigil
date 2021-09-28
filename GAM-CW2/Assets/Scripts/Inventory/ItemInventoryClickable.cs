using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventoryClickable : MonoBehaviour
{
    public IInventoryItem item;
    public Inventory inv;

    

    public void OnItemClicked()
    {
        if (item != null)
        {
            Debug.Log("Using: " + item.itemName);
            inv.useItem(item);
        }
    }
}
