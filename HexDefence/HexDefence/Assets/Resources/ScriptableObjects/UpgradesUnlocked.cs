using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesUnlocked", menuName = "ScriptableObjects/UpgradesUnlocked", order = 22)]
public class UpgradesUnlocked : ScriptableObject
{

    public List<Upgrade> upgradesUnlocked;

    [Button("Add All Upgrades")]
    public void AddAllUpgrades()
    {
        foreach (Upgrade upgrade in Resources.LoadAll<Upgrade>("ScriptableObjects/Upgrade"))
        {
            if (ArePrerequisitesMet(upgrade))
            {
                upgradesUnlocked.Add(upgrade);
            }
        }
    }

    private bool ArePrerequisitesMet(Upgrade upgrade)
    {
        // If prerequisites list is null, return true
        if (upgrade.prerequisites == null)
        {
            return true;
        }

        foreach (Upgrade prerequisite in upgrade.prerequisites)
        {
            // Check if the prerequisite is unlocked and if all its prerequisites are met
            if (!prerequisite.isUnlocked || !ArePrerequisitesMet(prerequisite))
            {
                return false;
            }
        }
        return true;
    }

    [Button("Remove All Upgrades")]
    public void RemoveAllUpgrades()
    {
        upgradesUnlocked.Clear();
    }

    public void UnlockUpgrade(Upgrade upgrade)
    {
        if (!upgradesUnlocked.Contains(upgrade))
        {
            upgradesUnlocked.Add(upgrade);
        }
    }

    public int UnlockedTowerUpgradesCount(HexBuilding hexBuilding)
    {
        int count = 0;
        foreach (Upgrade upgrade in upgradesUnlocked)
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
        upgradesUnlocked.Add(upgrade);
    }

}
