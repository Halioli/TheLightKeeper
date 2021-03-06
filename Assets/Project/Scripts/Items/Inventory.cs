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

    public bool gotChanged = false;

    // Private Attributes
    [SerializeField] protected int numberOfInventorySlots;
    protected int numberOfOccuppiedInventorySlots;
    protected bool inventoryIsEmpty;

    [SerializeField] protected int maxNumberOfSlots;


    private Inventory otherInventory = null;


    // Action
    public delegate void InventoryAction();
    public static event InventoryAction OnItemMove;
    public static event InventoryAction OnItemMoveFail;


    // Initializer Methods
    //public void Awake()
    //{
        
    //}

    private void Start()
    {
        Init();
    }

    protected void Init()
    {
        numberOfOccuppiedInventorySlots = 0;
        indexOfSelectedInventorySlot = 0;
        inventoryIsEmpty = true;

        InitInventory();
    }


    public virtual void InitInventory()
    {
        inventory.Clear();
        for (int i = 0; i < numberOfInventorySlots; ++i)
        {
            inventory.Add(Instantiate(emptyStack, transform));
        }
        gotChanged = true;
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
        int i = numberOfInventorySlots-1;
        while (i >= 0)
        {
            if (inventory[i].StackContainsItem(itemToCompare))
            {
                return i;
            }
            --i;
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

    public bool ItemCanBeAdded(Item itemToCompare)
    {
        return (NextEmptyInventorySlot() != -1) ||
               (NextInventorySlotWithAvailableSpaceToAddItem(itemToCompare) != -1);
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
        while (i < numberOfInventorySlots)
        {
            if (inventory[i].StackContainsItem(itemToCompare))
            {
                amountInInventory += inventory[i].GetAmountInStack();
            }
            i++;
        }

        hasEnough = amountInInventory >= requiredAmount;

        return hasEnough;
    }

    public bool InventoryContainsItemAndAmount(Item itemToCompare, int requiredAmount, out int amountInInventory)
    {
        bool hasEnough = false;
        int i = 0;
        amountInInventory = 0;
        while (i < numberOfInventorySlots)
        {
            if (inventory[i].StackContainsItem(itemToCompare))
            {
                amountInInventory += inventory[i].GetAmountInStack();
            }
            ++i;
        }
        
        hasEnough = amountInInventory >= requiredAmount;

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

        if (couldAddItem)
        {
            gotChanged = true;
        }

        return couldAddItem;
    }


    public bool AddItemToInventory(Item itemToAdd, out int outStackIndex)
    {
        bool couldAddItem = false;

        // Check if the inventory is empty, to add item directly
        if (InventoryIsEmpty())
        {
            inventory[0].InitStack(itemToAdd);

            numberOfOccuppiedInventorySlots++;
            inventoryIsEmpty = false;
            couldAddItem = true;
            outStackIndex = 0;
        }
        else
        {
            outStackIndex = NextInventorySlotWithAvailableSpaceToAddItem(itemToAdd);
            if (outStackIndex != -1)
            {
                // Add to slot in use
                inventory[outStackIndex].AddOneItemToStack();
                couldAddItem = true;
            }
            else
            {
                outStackIndex = NextEmptyInventorySlot();
                if (outStackIndex != -1)
                {
                    inventory[outStackIndex].InitStack(itemToAdd);
                    numberOfOccuppiedInventorySlots++;
                    couldAddItem = true;
                }
            }
        }

        if (couldAddItem)
        {
            gotChanged = true;
        }

        return couldAddItem;
    }



    public bool AddNItemsToInventory(Item itemToAdd, int numberOfItemsToAdd)
    {
        for (int i = 0; i < numberOfItemsToAdd; ++i)
        {
            if (!AddItemToInventory(itemToAdd))
            {
                return false;
            }
        }
        return true;
    }


    public bool SubstractItemFromInventory(Item itemToSubstract)
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

        if (couldRemoveItem)
        {
            gotChanged = true;
        }
    
        return couldRemoveItem;
    }

    public bool SubstractNItemsFromInventory(Item itemToSubstract, int numberOfItemsToSubstract)
    {
        for (int i = 0; i < numberOfItemsToSubstract; ++i)
        {
            if (!SubstractItemFromInventory(itemToSubstract))
            {
                return false;
            }
        }
        return true;
    }


    // Call only if can substract items, need previous check
    public Dictionary<int, int> GetDataAndSubstractNItemsFromInventory(Item itemToSubstract, int numberOfItemsToSubstract)
    {
        // key: stackIndex
        // value: substracted stack amount
        Dictionary<int, int> data = new Dictionary<int, int>();

        while (numberOfItemsToSubstract > 0)
        {
            // key: stackIndex
            // value: amount substracted from stack
            KeyValuePair<int, int> stackData = GetStackDataAndSubstractItemFromInventory(itemToSubstract, numberOfItemsToSubstract);
            data.Add(stackData.Key, stackData.Value);

            numberOfItemsToSubstract -= stackData.Value;
        }

        return data;
    }

    public KeyValuePair<int, int> GetStackDataAndSubstractItemFromInventory(Item itemToSubstract, int numberOfItemsToSubstract)
    {
        // Check get next stack index with item
        int index = NextInventorySlotWithAvailableItemToSubstract(itemToSubstract);

        int amountInStack = inventory[index].GetAmountInStack();

        // if stack contains less than numberOfItemsToSubstract, only substract amountInStack
        // else substract numberOfItemsToSubstract
        int amountToSubstract = amountInStack < numberOfItemsToSubstract ? amountInStack : numberOfItemsToSubstract;


        if (index != -1)
        {
            for (int i = 0; i < amountToSubstract; ++i)
            {
                SubstractItemFromInventorySlot(index);
            }
        }


        // key: stackIndex
        // value: amount substracted from stack
        KeyValuePair<int, int> stackData = new KeyValuePair<int, int>(index, amountToSubstract);

        return stackData;
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

        gotChanged = true;
    }


    // Call ONLY IF can substract all items
    public void ProgressiveSubstractNItemsFromInventory(Item[] itemsToSubstract, int[] amountsToSubstract)
    {
        // Find biggest amount to sbstract
        int biggestAmount = amountsToSubstract[0];
        for (int i = 1; i < amountsToSubstract.Length; ++i)
        {
            biggestAmount = biggestAmount < amountsToSubstract[i] ? amountsToSubstract[i] : biggestAmount;
        }


        StartCoroutine(ProgressivelySubstractItems(itemsToSubstract, amountsToSubstract, biggestAmount));
    }

    IEnumerator ProgressivelySubstractItems(Item[] itemsToSubstract, int[] amountsToSubstract, int biggestAmount)
    {
        for (int count = 0; count < biggestAmount; ++count)
        {
            for (int i = 0; i < amountsToSubstract.Length; ++i)
            {
                if (amountsToSubstract[i] > 0)
                {
                    SubstractItemFromInventory(itemsToSubstract[i]);
                    --amountsToSubstract[i];
                }
            }

            yield return new WaitForSeconds(0.02f);

        }


    }



    // Other Methods
    public List<ItemStack.itemStackToDisplay> Get3ItemsToDisplayInHUD()
    {
        int i = indexOfSelectedInventorySlot;
        int n = numberOfInventorySlots;

        List<ItemStack.itemStackToDisplay> itemsToDisplay = new List<ItemStack.itemStackToDisplay>();

        if (((i - 1) % n) < 0)
        {
            itemsToDisplay.Add(inventory[3].GetStackToDisplay());
        }
        else
        {
            itemsToDisplay.Add(inventory[(i - 1) % n].GetStackToDisplay());
        }
        itemsToDisplay.Add(inventory[i].GetStackToDisplay());
        itemsToDisplay.Add(inventory[(i + 1) % n].GetStackToDisplay());

        return itemsToDisplay;
    }

    public void SetSelectedInventorySlotIndex(int index)
    {
        indexOfSelectedInventorySlot = index;
    }


    public void MoveItemToOtherInventory()
    {
        if (otherInventory == null) return;
        if (inventory[indexOfSelectedInventorySlot].itemInStack == itemNull)
        {
            if (OnItemMoveFail != null) OnItemMoveFail();
            return;
        }


        bool canSwap = otherInventory.ItemCanBeAdded(inventory[indexOfSelectedInventorySlot].itemInStack);

        if (canSwap)
        {
            otherInventory.AddItemToInventory(inventory[indexOfSelectedInventorySlot].itemInStack);
            SubstractItemFromInventorySlot(indexOfSelectedInventorySlot);

            otherInventory.gotChanged = true;

            if (OnItemMove != null) OnItemMove();
        }
        else
        {
            if (OnItemMoveFail != null) OnItemMoveFail();
        }

    }

    public void SetOtherInventory(Inventory otherInventory)
    {
        this.otherInventory = otherInventory;
    }

    public Dictionary<int,int> GetInventoryData()
    {
        Dictionary<int, int> inventoryData = new Dictionary<int, int>();

        for(int i = 0; i < inventory.Count; i++)
        {
            if(!inventory[i].StackIsEmpty())
            {
                if (inventoryData.ContainsKey(inventory[i].itemInStack.ID))
                {
                    inventoryData[inventory[i].itemInStack.ID] += inventory[i].amountInStack;
                }
                else
                {
                    inventoryData[inventory[i].itemInStack.ID] = inventory[i].amountInStack;
                }
            }
        }
        return inventoryData;
    }


}
