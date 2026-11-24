using System;
using UnityEngine;
public class PlayerXp : MonoBehaviour
{
    public int currentLevel;
    public float currentXp;
    public int xpRequired;
    public void AddXp(int xp)
    {
        currentXp += xp * SceneContext.Player.Stats.GetStat(PlayerStats.StatKey.XPMultiplier).Value;
        onXpGain?.Invoke(currentXp);
        if (currentXp >= xpRequired)
        {
            currentXp = currentXp - xpRequired;
            currentLevel += 1;
            onLevelUp?.Invoke(currentLevel);
        }
    }
    public event Action<int> onLevelUp;
    public event Action<float> onXpGain;
}