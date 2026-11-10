using UnityEngine;

public abstract class SOUpgradeDefinition : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public SOUpgradeDefinition[] requires;   // all must be owned before this can appear
    public int maxStacks = 1;
    public int weight = 1;                 // roll weight
    public abstract void Apply(Player player);
}