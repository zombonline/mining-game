using System;
using UnityEngine;
public class PlayerXp : MonoBehaviour
{
    public int currentLevel;
    public int currentXp;
    public int xpRequired;
    public void AddXp(int xp)
    {
        currentXp += xp;
        onXpGain?.Invoke(currentXp);
        if (currentXp >= xpRequired)
        {
            currentXp = currentXp - xpRequired;
            currentLevel += 1;
            onLevelUp?.Invoke(currentLevel);
        }
    }
    public event Action<int> onLevelUp;
    public event Action<int> onXpGain;
}