using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    // Private Attributes
    private const int MAX_LEVEL = 5;
    private int currentLevel;

    private Inventory playerInventory;
    private Dictionary<Item, int> playerInventoryItems;
    private int numberOfEmptySlotsInPlayerInventory;

    private Vector2 droppedItemPosition;

    // Public Attributes
    public List<RecepieCollection> recepiesLvl;
    public List<Recepie> availableRecepies;
    // public ParticleSystem[] craftingParticles;

    //Events

    public delegate void CraftAction();
    public static event CraftAction OnCrafting;

    void Start()
    {
        currentLevel = 1;
        InitAllRecepies();

        availableRecepies = new List<Recepie>();
        AddAvailableRecepies();

        droppedItemPosition = new Vector2(transform.position.x, transform.position.y - 1f);

        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();
        playerInventoryItems = new Dictionary<Item, int>();


        //foreach (ParticleSystem particle in craftingParticles)
        //{
        //    particle.Stop();
        //}
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RecepieWasSelected(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RecepieWasSelected(1);
        }
    }


    private void OnEnable()
    {
        CraftableItemButton.OnClickedRecepieButton += RecepieWasSelected;
        CoreUpgrade.OnCoreUpgrade += LevelUp;
    }

    private void OnDisable()
    {
        CraftableItemButton.OnClickedRecepieButton -= RecepieWasSelected;
        CoreUpgrade.OnCoreUpgrade -= LevelUp;
    }

    private void InitAllRecepies()
    {
        foreach (RecepieCollection recepieCollection in recepiesLvl)
        {
            recepieCollection.InitRecepies();
        }
    }


    private void AddAvailableRecepies()
    {
        foreach (Recepie recepie in recepiesLvl[currentLevel - 1].recepies)
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
        playerInventoryItems.Clear();
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

    private bool PlayerHasEnoughItemsToCraftRecepie(Recepie recepieToCraft)
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

    private void RemoveRecepieRequiredItems(Recepie recepieToCraft)
    {
        foreach (KeyValuePair<Item, int> requiredItem in recepieToCraft.requiredItems)
        {
            playerInventory.SubstractNItemsFromInventory(requiredItem.Key, requiredItem.Value);
        }
    }

    private void AddRecepieResultingItems(Recepie recepieToCraft)
    {
        for (int i = 0; i < recepieToCraft.resultingItem.Value; ++i)
        {
            if (!playerInventory.AddItemToInventory(recepieToCraft.resultingItem.Key))
            {
                // instantiate item in map instead
                GameObject item = Instantiate(recepieToCraft.resultingItem.Key.prefab, droppedItemPosition, Quaternion.identity);
                item.GetComponent<ItemGameObject>().DropsRandom();
            }
        }
        
    }


    public void RecepieWasSelected(int selectedRecepieIndex)
    {
        UpdatePlayerInventoryData();
        if (PlayerHasEnoughItemsToCraftRecepie(availableRecepies[selectedRecepieIndex]))
        {
            OnCrafting();
            RemoveRecepieRequiredItems(availableRecepies[selectedRecepieIndex]);
            AddRecepieResultingItems(availableRecepies[selectedRecepieIndex]);
        }
        else
        {
            Debug.Log("Cannot craft " + availableRecepies[selectedRecepieIndex].recepieName);
        }
    }

   /* IEnumerator CraftingParticleSystem()
    {
        foreach (ParticleSystem particle in craftingParticles)
        {
            particle.Play();
        }

        yield return new WaitForSeconds(3.4f);

        foreach (ParticleSystem particle in craftingParticles)
        {
            particle.Play();
        }
    }*/
}
