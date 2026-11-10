using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] private SOUpgradeDefinition[] upgrades;
    private Dictionary<SOUpgradeDefinition, int> unlockedUpgrades = new Dictionary<SOUpgradeDefinition, int>();
    List<SOUpgradeDefinition> previousUpgrades;

    public List<SOUpgradeDefinition> GetUnlocks(int amount)
    {
        List<SOUpgradeDefinition> result = new List<SOUpgradeDefinition>();
        for(int i = 0; i < amount; i++)
        {
            int stacks;
            var possibleUpgrades = upgrades.Where(upgrade => 
                (!unlockedUpgrades.TryGetValue(upgrade, out stacks) ||
                stacks <= upgrade.maxStacks) &&
                !result.Contains(upgrade)
                ).ToList();
            Debug.Log("Possible upgrade length: " + possibleUpgrades.Count);
            float maxWeight = possibleUpgrades.Sum(upgrade => upgrade.weight);
            float random = Random.Range(0, maxWeight);
            float currentWeight = 0;
            foreach(var upgrade in possibleUpgrades)
            {
                if (previousUpgrades != null && previousUpgrades.Contains(upgrade))
                    currentWeight += upgrade.weight / 2;
                else
                    currentWeight += upgrade.weight;
                if (currentWeight >= random)
                {
                    result.Add(upgrade);
                    break;
                }
            }
        }
        previousUpgrades = result;
        return result;
    }

    public void SetUsed(SOUpgradeDefinition upgrade)
    {
        unlockedUpgrades[upgrade] += 1;
    }

}
