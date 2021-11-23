using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    // Private Attributes
    private const int MAX_LEVEL = 5;
    private int currentLevel;
    private List<RecepieGameObject> availableRecepies;

    private Inventory playerInventory;
    private Dictionary<Item, int> playerInventoryItems;
    private int numberOfEmptySlotsInPlayerInventory;

    private Vector2 droppedItemPosition;

    // Public Attributes
    public List<RecepieCollection> recepiesLvl;


    void Start()
    {
        currentLevel = 1;

        availableRecepies = new List<RecepieGameObject>();
        AddAvailableRecepies();

        droppedItemPosition = new Vector2(transform.position.x, transform.position.y - 1f);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelUp();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RecepieWasSelected(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RecepieWasSelected(1);
        }
    }




    private void AddAvailableRecepies()
    {
        foreach (RecepieGameObject recepie in recepiesLvl[currentLevel - 1].recepies)
        {
            availableRecepies.Add(recepie);
        }
    }
    
    public void LevelUp()
    {
        if (currentLevel < MAX_LEVEL)
        {
            ++currentLevel;
            AddAvailableRecepies();
        }
    }

    private void UpdatePlayerInventoryData()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();

        playerInventoryItems = new Dictionary<Item, int>();
        numberOfEmptySlotsInPlayerInventory = 0;

        foreach (ItemStack playerInventoryItemStack in playerInventory.inventory)
        {
            if (playerInventoryItemStack.StackIsEmpty())
            {
                ++numberOfEmptySlotsInPlayerInventory;
            }
            else
            {
                if (!playerInventoryItems.ContainsKey(playerInventoryItemStack.itemInStack))
                {
                    playerInventoryItems[playerInventoryItemStack.itemInStack] = 0;
                }
                playerInventoryItems[playerInventoryItemStack.itemInStack] += playerInventoryItemStack.amountInStack;
            }
        }

    }

    private bool PlayerHasEnoughItemsToCraftRecepie(RecepieGameObject recepieToCraft)
    {
        foreach (KeyValuePair<Item, int> requiredItem in recepieToCraft.requiredItems)
        {
            if (!playerInventory.InventoryContainsItemAndAmount(requiredItem.Key, requiredItem.Value))
            {
                return false;
            }
        }
        return true;
    }

    private void RemoveRecepieRequiredItems(RecepieGameObject recepieToCraft)
    {
        foreach (KeyValuePair<Item, int> requiredItem in recepieToCraft.requiredItems)
        {
            playerInventory.SubstractNItemsFromInventory(requiredItem.Key, requiredItem.Value);
        }
    }

    private void AddRecepieResultingItems(RecepieGameObject recepieToCraft)
    {
        for (int i = 0; i < recepieToCraft.resultingItem.Value; ++i)
        {
            if (!playerInventory.AddItemToInventory(recepieToCraft.resultingItem.Key))
            {
                // instantiate item in map instead
                Instantiate(recepieToCraft.resultingItem.Key, droppedItemPosition, Quaternion.identity);
            }
        }
        
    }

    public void RecepieWasSelected(int selectedRecepieIndex)
    {
        UpdatePlayerInventoryData();
        if (PlayerHasEnoughItemsToCraftRecepie(availableRecepies[selectedRecepieIndex]))
        {
            RemoveRecepieRequiredItems(availableRecepies[selectedRecepieIndex]);
            AddRecepieResultingItems(availableRecepies[selectedRecepieIndex]);
        }
        else
        {
            Debug.Log("Cannot craft " + availableRecepies[selectedRecepieIndex].recepie.recepieName);
        }
    }

    
}
