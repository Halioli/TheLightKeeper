using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradesSystem : MonoBehaviour
{
    [SerializeField] UpgradeMenuCanvas upgradeMenuCanvas;
    [SerializeField] UpgradesDataSaver upgardesDataSaver;
    [SerializeField] TranslationItemSpawner translationItemSpawner;

    private Dictionary<Item, int> playerInventoryItems;
    HotbarInventory playerInventory;
    [SerializeField] public List<UpgradeBranch> upgradeBranches;

    // Events
    public delegate void UpgardeAction();
    public static event UpgardeAction OnUpgrade;
    public static event UpgardeAction OnUpgradeFail;


    private bool canUpgrade = false;

    private Vector2 currentNodeTransformPosition;


    private void Awake()
    {
        playerInventoryItems = new Dictionary<Item, int>();


        for (int i = 0; i < upgradeBranches.Count; ++i)
        {
            upgradeBranches[i].Init(i);
        }


        int[] allLastCompletedButtonIndex = upgardesDataSaver.GetLoadedUpgradesData();
        if (allLastCompletedButtonIndex == null)
        {
            upgradeMenuCanvas.FirstTimeSetAllLastCompletedButtonIndex();
        }
        else
        {
            upgradeMenuCanvas.SetAllLastCompletedButtonIndex(allLastCompletedButtonIndex);
        }
        
    }


    private void OnEnable()
    {
        UpgradeButton.OnUpgradeNodeHover += SetCurrentNodeTransformPosition;
        PauseMenu.OnGameExit += SaveUpgardesData;
    }
    private void OnDisable()
    {
        UpgradeButton.OnUpgradeNodeHover -= SetCurrentNodeTransformPosition;
        PauseMenu.OnGameExit += SaveUpgardesData;
    }



    public void Init(HotbarInventory playerInventory)
    {
        this.playerInventory = playerInventory;
    }


    // 1st Test upgrade to check // Tested on button hover
    public void UpgradeBranchIsTested (int upgradeBranchIndex, int upgradeIndex, int[] amountsInInventory)
    {
        if (upgradeBranches[upgradeBranchIndex].IsCompleted()) return;

        UpdatePlayerInventoryData();

        canUpgrade = PlayerHasEnoughItemsToUpgrade(upgradeBranches[upgradeBranchIndex].upgrades[upgradeIndex], amountsInInventory);
    }

    // 2nd Upgrade can be selected
    public bool UpgradeBranchIsSelected(int index)
    {
        if (upgradeBranches[index].IsCompleted()) return false;

        if (canUpgrade)
        {
            RemoveUpgradeRequiredItems(upgradeBranches[index].GetCurrentUpgrade());

            upgradeBranches[index].Upgrade();

            if (OnUpgrade != null) OnUpgrade();

            return true;
        }


        if (OnUpgradeFail != null) OnUpgradeFail();
        
        return false;
    }




    public void AlwaysCompleteUpgradeBranchIsSelected(int index)
    {
        if (upgradeBranches[index].IsCompleted()) return;


        upgradeBranches[index].Upgrade();
        //if (OnUpgrade != null) OnUpgrade();
    }

    public void UpdatePlayerInventoryData()
    {
        playerInventoryItems.Clear();

        foreach (ItemStack playerInventoryItemStack in playerInventory.inventory)
        {
            if (!playerInventoryItemStack.StackIsEmpty())
            {
                if (!playerInventoryItems.ContainsKey(playerInventoryItemStack.itemInStack))
                {
                    playerInventoryItems[playerInventoryItemStack.itemInStack] = 0;
                }
                playerInventoryItems[playerInventoryItemStack.itemInStack] += playerInventoryItemStack.amountInStack;
            }
        }

    }

    public bool PlayerHasEnoughItemsToUpgrade(Upgrade upgrade, int[] amountsInInventory)
    {
        int i = 0;
        bool hasEnoughItems = true;

        foreach (KeyValuePair<Item, int> requiredItem in upgrade.requiredItems)
        {
            if (!playerInventory.InventoryContainsItemAndAmount(requiredItem.Key, requiredItem.Value, out amountsInInventory[i]))
            {
                hasEnoughItems = false; 
            }

            ++i;
        }
        return hasEnoughItems;
    }

    private void RemoveUpgradeRequiredItems(Upgrade upgrade)
    {
        // Method 1
        //foreach (KeyValuePair<Item, int> requiredItem in upgrade.requiredItems)
        //{
        //    playerInventory.SubstractNItemsFromInventory(requiredItem.Key, requiredItem.Value);
        //}


        // Method 2
        //playerInventory.ProgressiveSubstractNItemsFromInventory(upgrade.requiredItemsList.ToArray(), upgrade.requiredAmountsList.ToArray());


        // Method 3
        foreach (KeyValuePair<Item, int> requiredItem in upgrade.requiredItems)
        {
            // key: stackIndex
            // value: substracted stack amount
            Dictionary<int, int> data = playerInventory.GetDataAndSubstractNItemsFromInventory(requiredItem.Key, requiredItem.Value);

            foreach (KeyValuePair<int, int> stackData in data)
            {
                // 1st get player inventory stack position
                Vector2 stackTransformPosition = playerInventory.GetStackTransformPosition(stackData.Key);

                // 2nd get upgrade node position
                // --> set via UpgradeButton event to currentNodeTransformPosition attribute variable

                // 3rd call TranslationItemSpawner Spawn()
                // build KeyValuePair with
                //  key: item 
                //  value: subtracted amount from the stack

                translationItemSpawner.Spawn(new KeyValuePair<Item, int>(requiredItem.Key, stackData.Value), stackTransformPosition, currentNodeTransformPosition);
            }

        }
        

    }

    public void DoOnUpgardeFail()
    {
        if (OnUpgradeFail != null) OnUpgradeFail();
    }


    public bool UpgradeBranchIsCompleted(int upgradeBranchIndex)
    {
        return upgradeBranches[upgradeBranchIndex].IsCompleted();
    }



    private void SetCurrentNodeTransformPosition(Vector2 currentNodeTransformPosition)
    {
        this.currentNodeTransformPosition = currentNodeTransformPosition;
    }



    private void SaveUpgardesData()
    {
        upgardesDataSaver.SaveUpgradesData(upgradeMenuCanvas.GetAllUpgardesLastActiveButtonIndex());
    }

}
