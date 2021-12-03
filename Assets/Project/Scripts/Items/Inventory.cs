using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Inventory : MonoBehaviour
{
    // Public Attributes
    public Item itemNull;
    public ItemStack emptyStack;

    public List<ItemStack> inventory = new List<ItemStack>();

    public int indexOfSelectedInventorySlot;



    // Private Attributes
    private int numberOfInventorySlots;
    private int numberOfOccuppiedInventorySlots;
    private bool inventoryIsEmpty;

    private const int MAX_NUMBER_OF_SLOTS = 9;



    // Initializer Methods
    public void Start()
    {
        numberOfInventorySlots = 4;
        numberOfOccuppiedInventorySlots = 0;
        indexOfSelectedInventorySlot = 0;
        inventoryIsEmpty = true;


        InitInventory();
    }

    public void InitInventory()
    {
        for (int i = 0; i < numberOfInventorySlots; i++)
        {
            inventory.Add(Instantiate(emptyStack, transform));
        }
        
    }


    // Getter Methods
    public int GetInventorySize() { return numberOfInventorySlots; }

    public int NextInventorySlotWithAvailableSpaceToAddItem(Item itemToCompare)
    {
        int i = 0;
        while (i < numberOfInventorySlots)
        {
            if (inventory[i].StackContainsItem(itemToCompare) && inventory[i].StackHasSpaceLeft())
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public int NextInventorySlotWithAvailableItemToSubstract(Item itemToCompare)
    {
        int i = 0;
        while (i < numberOfInventorySlots)
        {
            if (inventory[i].StackContainsItem(itemToCompare))
            {
                return i;
            }
            i++;
        }
        return -1;
    }


    public int NextEmptyInventorySlot()
    {
        int i = 0;
        while (i < numberOfInventorySlots)
        {
            if (inventory[i].StackHasNoItemsLeft())
            {
                return i;
            }
            i++;
        }
        return -1;
    }



    // Modifier Methods
    public void UpgradeInventory()
    {
        if (numberOfInventorySlots < MAX_NUMBER_OF_SLOTS)
        {
            numberOfInventorySlots++;
            inventory.Add(Instantiate(emptyStack, transform));
        }
    }


    // Bool Methods
    public bool InventoryIsEmpty()
    {
        return inventoryIsEmpty;
    }

    public bool InventoryContainsItem(Item itemToCompare)
    {
        bool wasFound = false;
        int i = 0;
        while (!wasFound && i < numberOfInventorySlots)
        {
            wasFound = inventory[i].StackContainsItem(itemToCompare);
            i++;
        }
        return wasFound;
    }

    public bool InventoryContainsItemAndAmount(Item itemToCompare, int requiredAmount)
    {
        bool hasEnough = false;
        int i = 0;
        int amountInInventory = 0;
        while (!hasEnough && i < numberOfInventorySlots)
        {
            if (inventory[i].StackContainsItem(itemToCompare))
            {
                amountInInventory += inventory[i].GetAmountInStack();
            }
            hasEnough = amountInInventory >= requiredAmount;
            i++;
        }
        return hasEnough;
    }


    public bool AddItemToInventory(Item itemToAdd)
    {
        bool couldAddItem = false;

        // Check if the inventory is empty, to add item directly
        if (InventoryIsEmpty())
        {
            inventory[0].InitStack(itemToAdd);

            numberOfOccuppiedInventorySlots++;
            inventoryIsEmpty = false;
            couldAddItem = true;
        }
        else
        {
            int index = NextInventorySlotWithAvailableSpaceToAddItem(itemToAdd);
            if (index != -1)
            {
                // Add to slot in use
                inventory[index].AddOneItemToStack();
                couldAddItem = true;
            }
            else
            {
                index = NextEmptyInventorySlot();
                if (index != -1)
                {
                    inventory[index].InitStack(itemToAdd);
                    numberOfOccuppiedInventorySlots++;
                    couldAddItem = true;
                }
            }
        }

        return couldAddItem;
    }


    public bool SubstractItemToInventory(Item itemToSubstract)
    {
        bool couldRemoveItem = false;

        // Check if the inventory contains an item to substract 
        int index = NextInventorySlotWithAvailableItemToSubstract(itemToSubstract);

        if (index != -1)
        {
            inventory[index].SubstractOneItemFromStack();

            // Set slot to empty if the last unit of the item was substracted
            if (inventory[index].StackHasNoItemsLeft())
            {
                inventory[index].InitEmptyNullStack(itemNull);
                numberOfOccuppiedInventorySlots--;

                if (numberOfOccuppiedInventorySlots == 0)
                {
                    inventoryIsEmpty = true;
                }
            }
            couldRemoveItem = true;
        }

        return couldRemoveItem;
    }


    public bool SubstractNItemsFromInventory(Item itemToSubstract, int numberOfItemsToSubstract)
    {
        for (int i = 0; i < numberOfItemsToSubstract; ++i)
        {
            if (!SubstractItemToInventory(itemToSubstract))
            {
                return false;
            }
        }
        return true;
    }

    public void SubstractItemFromInventorySlot(int inventorySlot)
    {
        inventory[inventorySlot].SubstractOneItemFromStack();

        // Set slot to empty if the last unit of the item was substracted
        if (inventory[inventorySlot].StackHasNoItemsLeft())
        {
            inventory[inventorySlot].InitEmptyNullStack(itemNull);
            numberOfOccuppiedInventorySlots--;

            if (numberOfOccuppiedInventorySlots == 0)
            {
                inventoryIsEmpty = true;
            }
        }
    }



    // Other Methods
    public List<ItemStack.itemStackToDisplay> Get3ItemsToDisplayInHUD()
    {
        int i = indexOfSelectedInventorySlot;
        int n = numberOfInventorySlots;

        List<ItemStack.itemStackToDisplay> itemsToDisplay = new List<ItemStack.itemStackToDisplay>();

        itemsToDisplay.Add(inventory[(i - 1) % n].GetStackToDisplay());
        itemsToDisplay.Add(inventory[i].GetStackToDisplay());
        itemsToDisplay.Add(inventory[(i + 1) % n].GetStackToDisplay());

        return itemsToDisplay;
    }


    public void CycleLeftSelectedItemIndex()
    {
        --indexOfSelectedInventorySlot;
        indexOfSelectedInventorySlot = indexOfSelectedInventorySlot < 0 ? indexOfSelectedInventorySlot = numberOfInventorySlots-1 : indexOfSelectedInventorySlot;

    }

    public void CycleRightSelectedItemIndex()
    {
        indexOfSelectedInventorySlot = (indexOfSelectedInventorySlot + 1) % numberOfInventorySlots;
    }


    public void UseSelectedConsumibleItem()
    {
        if (inventory[indexOfSelectedInventorySlot].itemInStack.itemType == ItemType.CONSUMIBLE)
        {
            inventory[indexOfSelectedInventorySlot].itemInStack.prefab.GetComponent<ItemGameObject>().DoFunctionality();
            SubstractItemFromInventorySlot(indexOfSelectedInventorySlot);
        }
    }
}
