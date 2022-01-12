using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuCanvas : MonoBehaviour
{
    [SerializeField] UpgradeButton[] upgradeButtons;
    public UpgradesSystem upgradesSystem;

    public void Init(List<UpgradeBranch> upgradeBranches)
    {
        int j = 0;

        for (int i = 0; i < upgradeBranches.Count; ++i)
        {
            upgradeBranches[i].Init(i);
            Upgrade upgrade = upgradeBranches[i].GetCurrentUpgrade();
            Sprite[] sprites = new Sprite[upgrade.requiredItems.Count];
            string[] amounts = new string[upgrade.requiredItems.Count];
            j = 0;

            foreach (KeyValuePair<Item, int> requiredItemPair in upgrade.requiredItems)
            {
                sprites[j] = requiredItemPair.Key.sprite;
                amounts[j] = requiredItemPair.Value.ToString();
                ++j;
            }
            upgradeButtons[i].UpdateButtonElements(upgrade.upgradeDescription, sprites, amounts);
        }
    }

    public void UpgradeBranchIsSelected(int index)
    {
        upgradesSystem.UpgradeBranchIsSelected(index);

        UpdateUpgradeButton(upgradesSystem.upgradeBranches, index);
    }

    private void UpdateUpgradeButton(List<UpgradeBranch> upgradeBranches, int index)
    {
        upgradeBranches[index].Init(index);

        Upgrade upgrade = upgradeBranches[index].GetCurrentUpgrade();
        Sprite[] sprites = new Sprite[upgrade.requiredItems.Count];
        string[] amounts = new string[upgrade.requiredItems.Count];
        int j = 0;

        foreach (KeyValuePair<Item, int> requiredItemPair in upgrade.requiredItems)
        {
            sprites[j] = requiredItemPair.Key.sprite;
            amounts[j] = requiredItemPair.Value.ToString();
            ++j;
        }
        upgradeButtons[index].UpdateButtonElements(upgrade.upgradeDescription, sprites, amounts);
    }
}
