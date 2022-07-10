using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class InventoryCounter : MonoBehaviour
    {
        public TMP_Text TextComponent;

        public int NumberOfCharacters;

        public CollectableInventory Inventory;
        private void OnEnable()
        {
            TextComponent.text = Inventory.Amount.ToString().PadLeft(NumberOfCharacters, '0');
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
}
