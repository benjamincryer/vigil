using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : MonoBehaviour, IInventoryItem //Acts as a gameobject and an inventory item
{
    public Sprite _itemImage;
    public string _itemName;

    public string itemName
    {
        get
        {
            return _itemName;
        }
    }

    public Sprite itemImage
    {
        get
        {
            return _itemImage;
        }
    }

    public void onPickup()
    {
        gameObject.SetActive(false); //deactives the object on pickup
    }

    public void onUse()
    {

    }
}
