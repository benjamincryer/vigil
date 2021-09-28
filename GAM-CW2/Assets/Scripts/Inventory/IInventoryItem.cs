using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string itemName { get; }
    Sprite itemImage { get; }
    void onPickup();
    void onUse();
}

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(IInventoryItem item)
    {
        this.item = item;
    }
    public IInventoryItem item;
}