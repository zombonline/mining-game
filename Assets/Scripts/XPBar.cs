using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;
    private void OnEnable()
    {
        Player.Instance.XP.onXpGain += HandleXPGain;
        Player.Instance.XP.onLevelUp += HandleLevelUp;
    }
    private void OnDisable()
    {
        Player.Instance.XP.onXpGain -= HandleXPGain;
        Player.Instance.XP.onLevelUp -= HandleLevelUp;
    }
    private void HandleXPGain(int currentXp)
    {
        xpSlider.value = (float)currentXp / Player.Instance.XP.xpRequired;
    }
    private void HandleLevelUp(int newLevel)
    {
        levelText.text = newLevel.ToString();
    }
}
