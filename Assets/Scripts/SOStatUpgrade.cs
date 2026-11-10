using UnityEngine;

[CreateAssetMenu(fileName = "StatUpgrade", menuName = "Scriptable Objects/StatUpgrade")]
public class SOStatUpgrade : SOUpgradeDefinition
{
    public PlayerStats.StatKey stat;
    public float amount;
    public ModifiedFloat.ModifierType type;
    public override void Apply(Player player) =>
        player.Stats.GetStat(stat).AddModifier(name, amount, type);
}
