using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textName, textDesc;
    [SerializeField] Image imageIcon;
    SOUpgradeDefinition upgrade;
    public static event Action buttonPressed;
    public void SetUpgrade(SOUpgradeDefinition upgrade)
    {
        this.upgrade = upgrade;
        textName.text = upgrade.displayName;
        textDesc.text = upgrade.description;
        imageIcon.sprite = upgrade.icon;
    }

    public void OnButtonPress()
    {
        upgrade.Apply(SceneContext.Player);
        buttonPressed?.Invoke();
    }

}
