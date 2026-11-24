using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;
    private void OnEnable()
    {
        SceneContext.Player.XP.onXpGain += HandleXPGain;
        SceneContext.Player.XP.onLevelUp += HandleLevelUp;
    }
    private void OnDisable()
    {
        SceneContext.Player.XP.onXpGain -= HandleXPGain;
        SceneContext.Player.XP.onLevelUp -= HandleLevelUp;
    }
    private void HandleXPGain(float currentXp)
    {
        xpSlider.value = currentXp / SceneContext.Player.XP.xpRequired;
    }
    private void HandleLevelUp(int newLevel)
    {
        levelText.text = newLevel.ToString();
    }
}
