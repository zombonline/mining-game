using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifiedFloat
{
    public enum ReplaceModifier
    {
        None,
        IfNewHigher,
        IfNewLower,
    }
    public enum ModifierType
    {
        Flat,
        Percent
    }
    [System.Serializable]
    public struct Modifier
    {
        public float value;
        public ModifierType type;

        public Modifier(float value, ModifierType type)
        {
            this.value = value;
            this.type = type;
        }

    }
    [SerializeField] private float baseValue;
    [NaughtyAttributes.ReadOnly, SerializeField] private Dictionary<string, Modifier> modifiers = new Dictionary<string, Modifier>();
    [NaughtyAttributes.ReadOnly] public float Value;

    public ModifiedFloat(float baseValue)
    {
        this.baseValue = baseValue;
        SetModifiedValue();
    }

    private void SetModifiedValue()
    {
        float flatSum= 0;
        float percentMultiplier = 1;
        foreach(var mod in modifiers.Values)
        {
            if (mod.type == ModifierType.Flat)
                flatSum += mod.value;
            if (mod.type == ModifierType.Percent)
                percentMultiplier *= mod.value;
        }
        Value = (baseValue + flatSum) * percentMultiplier;
    }

    public void AddModifier(string key, float value, ModifierType type, ReplaceModifier replaceRule = ReplaceModifier.None)
    {
        if(HasModifier(key))
        {
            switch (replaceRule)
            {
                case ReplaceModifier.IfNewLower:
                    if (value >= modifiers[key].value)
                        return;
                    break;
                case ReplaceModifier.IfNewHigher:
                    if (value <= modifiers[key].value)
                        return;
                    break;
                case ReplaceModifier.None:
                    break;
            }
        }
        var newMod = new Modifier(value, type);
        if (HasModifier(key))
            modifiers[key] = newMod;
        else
            modifiers.Add(key, newMod);
        SetModifiedValue();
    }
    public void RemoveModifier(string key)
    {
        if (!HasModifier(key))
            return;
        modifiers.Remove(key);
        SetModifiedValue();
    }

    public bool HasModifier(string key) => modifiers.ContainsKey(key);

}
