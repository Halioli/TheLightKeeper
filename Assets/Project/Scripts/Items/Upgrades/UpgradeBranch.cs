using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewUpgradeBranch", menuName = "Upgrade System/UpgradeBranch")]

public class UpgradeBranch : ScriptableObject
{
    // Public Attributes
    public string upgradeBranchName;
    public delegate void UpgradeBranchAction(int branchIndex);
    public static event UpgradeBranchAction OnBranchComplete;

    // Private Attributes
    private int numberOfUpgrades;
    private int currentUpgrade;
    private int branchIndex;
    private bool branchCompleted;
    [SerializeField] List<Upgrade> upgrades;


    public void Init(int index)
    {
        numberOfUpgrades = upgrades.Capacity;
        currentUpgrade = 0;
        branchIndex = index;
        branchCompleted = false;

        foreach (Upgrade upgrade in upgrades)
        {
            upgrade.Init();
        }
    }

    public void Upgrade()
    {
        upgrades[currentUpgrade++].InvokeResultEvent();
        
        if (currentUpgrade >= numberOfUpgrades)
        {
            branchCompleted = true;
            if (OnBranchComplete != null)
                OnBranchComplete(branchIndex);
        }
    }

    public Upgrade GetCurrentUpgrade()
    {
        return upgrades[currentUpgrade];
    }

}