using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectDisplay : MonoBehaviour
{
    [SerializeField] private UpgradeButton[] upgradeButtons;
    [SerializeField] private RectTransform upgradeButtonContainer;

    private void OnEnable()
    {
        SceneContext.Player.XP.onLevelUp += HandlePlayerLevelUp;
        UpgradeButton.buttonPressed += HandleUpgradeButtonPressed;
    }
    private void OnDisable()
    {
        SceneContext.Player.XP.onLevelUp -= HandlePlayerLevelUp;
        UpgradeButton.buttonPressed -= HandleUpgradeButtonPressed;
    }

    private void HandlePlayerLevelUp(int level)
    {
        DisplayUpgrades(SceneContext.UpgradesManager.GetUnlocks(3));
    }
    public void DisplayUpgrades(List<SOUpgradeDefinition> upgrades)
    {
        GamePause.Push();
        for(int i = 0; i < upgrades.Count; i++)
        {
            upgradeButtons[i].SetUpgrade(upgrades[i]);
        }
        upgradeButtonContainer.gameObject.SetActive(true);
    }
    private void HandleUpgradeButtonPressed()
    {
        Debug.Log("Upgrade button pressed, handling");
        GamePause.Pop();
        upgradeButtonContainer.gameObject.SetActive(false);
    }
}
