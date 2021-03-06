using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneInteractStation : InteractStation
{
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject droneCanvasGameObject;
    [SerializeField] InventoryMenu droneInventoryMenu;

    private bool inventoryIsOpen = false;


    void Start()
    {
        droneCanvasGameObject.SetActive(inventoryIsOpen);
    }

    void Update()
    {
        if (playerInsideTriggerArea)
        {
            GetInput();            //Waits the input from InteractStation 
        }
        else
        {
            if (inventoryIsOpen)
            {
                CloseDroneInventory();
            }
        }
    }


    override public void StationFunction()
    {
        if (inventoryIsOpen)
        {
            CloseDroneInventory();
        }
        else
        {
            OpenDroneInventory();
        }
    }

    private void OpenDroneInventory()
    {
        DoOnInteractOpen();

        inventoryIsOpen = true;
        droneCanvasGameObject.SetActive(true);
        //droneInventoryMenu.UpdateInventory();

        playerInventory.SetOtherInventory(this.inventory);
        this.inventory.SetOtherInventory(playerInventory);

        PauseMenu.gameIsPaused = true;
    }

    private void CloseDroneInventory()
    {
        DoOnInteractClose();

        inventoryIsOpen = false;
        droneCanvasGameObject.SetActive(false);

        playerInventory.SetOtherInventory(null);
        this.inventory.SetOtherInventory(null);

        PauseMenu.gameIsPaused = false;
    }
}
