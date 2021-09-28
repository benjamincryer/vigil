using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHUD : MonoBehaviour
{
    public Inventory inv;

    void Start()
    {
        inv.ItemAdded += InventoryItemAdded;
    }

    private void InventoryItemAdded(object sender, InventoryEventArgs e)
    {
        Transform panel = transform.Find("InventoryUI");
        foreach (Transform slot in panel)
        {
            Image image = slot.GetComponent<Image>();
            ItemInventoryClickable button = slot.GetComponent<ItemInventoryClickable>();

            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.item.itemImage;
                button.item = e.item;

                break;
            }
        }
    }

}
