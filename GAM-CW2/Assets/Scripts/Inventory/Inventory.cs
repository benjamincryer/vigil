using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //Locally stored list of items
    List<IInventoryItem> items = new List<IInventoryItem>();

    //Event to broadcast for UI
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemUsed;

    public void addItem(IInventoryItem item)
    {
        items.Add(item);
        
        if (ItemAdded != null)
        {
            ItemAdded.Invoke(this, new InventoryEventArgs(item));
        }

        item.onPickup();
    }

    public void useItem(IInventoryItem item)
    {
        item.onUse();

        if (ItemUsed != null)
        {
            ItemUsed.Invoke(this, new InventoryEventArgs(item));
        }
    }



}
