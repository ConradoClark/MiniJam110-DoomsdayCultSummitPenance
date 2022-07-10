using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryCounter : MonoBehaviour
{
    public TMP_Text TextComponent;

    public int NumberOfCharacters;

    public CollectableInventory Inventory;
    private void OnEnable()
    {
        Inventory.OnCollect += Inventory_OnCollect;
    }

    private void OnDisable()
    {
        Inventory.OnCollect -= Inventory_OnCollect;
    }

    private void Inventory_OnCollect(int obj)
    {
        TextComponent.text = Inventory.Amount.ToString().PadLeft(NumberOfCharacters, '0');
    }
}
