    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentUpgradesPossibilities", menuName = "ScriptableObjects/CurrentUpgradesPossibilities", order = 22)]
public class UpgradesUnlocked : ScriptableObject
{
    /// <summary>
    /// Stores all upgrades that are unlocked in profile 
    /// AND
    /// have their prerequisists met so they can be purchased in a game. 
    /// Updated recursivly after each purchas to show new upgrades
    /// NOTE: the bool"isUnlockedInProfile" in the indevidual upgrade is across all games not just the current game
    /// </summary>
    public List<Upgrade> CurrentUpgradesPossibilities ;
    /// <summary>
    /// Stores all purchased unlocks for this game
    /// </summary>
     public List<Upgrade> CurrentUpgrades ;

    [Button("Initialize CurrentUpgradesPossibilities")]
    public void InitializeCurrentUpgradesUnlocked()
    {
        ClearAllPossibleUpgrades();
        ClearAllCurrentUpgrades();
        CurrentUpgradesPossibilities = new List<Upgrade>();
        CurrentUpgrades = new List<Upgrade>();

        foreach (Upgrade upgrade in Resources.LoadAll<Upgrade>("ScriptableObjects/Upgrade"))
        {
            if (upgrade.prerequisites.Count() == 0 && upgrade.IsUnlockedInProfile)
            {
                CurrentUpgradesPossibilities.Add(upgrade);
            }
        }
    }

    [Button("Update CurrentUpgradesPossibilities")]
    public void UpdateCurrentUpgradesUnlocked()
    {
        ClearAllPossibleUpgrades();
        foreach (Upgrade upgrade in Resources.LoadAll<Upgrade>("ScriptableObjects/Upgrade"))
        {
            if (ArePrerequisitesMet(upgrade))
            {
                if (!CurrentUpgrades.Contains(upgrade))
                CurrentUpgradesPossibilities.Add(upgrade);
                else
                continue;
            }
        }
    }

    private bool ArePrerequisitesMet(Upgrade upgrade)
    {
        // If the upgrade is not unlocked, return false
        if (!upgrade.IsUnlockedInProfile)
        {
            return false;
        }

        // If the upgrade has prerequisites, check if all prerequisites are met
        if (upgrade.prerequisites != null && upgrade.prerequisites.Count() > 0)
        {
            foreach (Upgrade prerequisite in upgrade.prerequisites)
            {
                // If any prerequisite is not met or not in CurrentUpgrades, return false
                if (!ArePrerequisitesMet(prerequisite) || !CurrentUpgrades.Contains(prerequisite))
                {
                    return false;
                }
            }
        }

        // If the upgrade is unlocked and either has no prerequisites or all prerequisites are met and in CurrentUpgrades, return true
        return true;
    }

    [Button("Clear All CurrentUpgradesPossibilities")]
    public void ClearAllPossibleUpgrades()
    {
        CurrentUpgradesPossibilities?.Clear();
    }

    [Button("Clear All CurrentUpgrades")]
    public void ClearAllCurrentUpgrades()
    {
        CurrentUpgrades?.Clear();
    }

    public int UnlockedTowerUpgradesCount(HexBuilding hexBuilding)
    {
        int count = 0;
        foreach (Upgrade upgrade in CurrentUpgradesPossibilities)
        {
            if (upgrade.upgradeType == UpgradeType.Tower && upgrade._building == hexBuilding)
            {
                count++;
            }
        }
        return count;
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        CurrentUpgrades.Add(upgrade);
        UpdateCurrentUpgradesUnlocked();
        GameManager.Instance.UpdateUpgradesToAdd(-1);
    }

}
