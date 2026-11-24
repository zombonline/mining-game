using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool usePlayerStats; 
    [SerializeField] private float currentHealth, maxHealth, healthRegenRate;
    public event Action onDeath;
    public event Action<float> onCurrentHealthUpdated;
    bool dead = false;

    private void Awake()
    {
        currentHealth = usePlayerStats ? SceneContext.Player.Stats.GetStat(PlayerStats.StatKey.HealthMax).Value : maxHealth;
    }

    private void Update()
    {
        var rate = usePlayerStats ? SceneContext.Player.Stats.GetStat(PlayerStats.StatKey.HealthRegen).Value : healthRegenRate;
        currentHealth += rate * Time.deltaTime;
        onCurrentHealthUpdated?.Invoke(currentHealth);
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        onCurrentHealthUpdated?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (dead) return;
        dead = true;
        onDeath?.Invoke();
    }

    public float GetCurrentHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

}
