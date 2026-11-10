using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public enum StatKey
    {
        PickaxeSwingArc,
        PickaxeDamage,
        PickaxeReach,
        PickaxeCooldown,

        DamageBase,

        MoveSpeed,
        MoveSpeedMining,

        HealthMax,
        HealthRegen,

        XPMultiplier,
        XPMagnetiseDistance
    }

    [SerializeField] private Dictionary<StatKey, ModifiedFloat> stats = new Dictionary<StatKey, ModifiedFloat>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (StatKey key in System.Enum.GetValues(typeof(StatKey)))
        {
            if (!stats.ContainsKey(key))
                stats.Add(key, new ModifiedFloat(1f));

        }
        var stale = stats.Keys.Where(k => !System.Enum.IsDefined(typeof(StatKey), k)).ToList();
        foreach (var k in stale) stats.Remove(k);
        if (Application.isPlaying)
            foreach (var s in stats.Values) s.Reinitialize();
    }
#endif
    public ModifiedFloat GetStat(StatKey key) => stats[key];
}
