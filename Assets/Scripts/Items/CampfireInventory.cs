using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class CampfireInventory : BlockInventory
    {

        public InventorySlot fuelSlot;
        public InventorySlot burnableSlot;
        public InventorySlot outputSlot;

        public override void OnSetupFinished()
        {
            //map the slots to part of the inventory
            fuelSlot = inventorySlots[0];
            burnableSlot = inventorySlots[1];
            outputSlot = inventorySlots[2];

        }

        private void Update()
        {
            //check if we need to smelt
        }


    }
}