using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
        Flat,     // added to the base value
        Percent   // multiplier (1.1 = +10%, 0.5 = half), multiplied together
    }


    [Serializable]
    public class Modifier
    {
        public ModifierType type;
        [SerializeField] private float value;
        [SerializeField] private bool active = true;
        

        // Runtime-only links. Not serialized: Unity would store a copy of the
        // source stat rather than a reference to the live one.
        [NonSerialized] private ModifiedFloat source;
        [NonSerialized] private Action onChanged;

        public bool Active => active;
        public float Value => source != null ? source.Value : value;

        public Modifier(float value, ModifierType type, ModifiedFloat source = null)
        {
            this.value = value;
            this.type = type;
            this.source = source;
        }

        public void SetActive(bool a)
        {
            if (active == a) return;
            active = a;
            onChanged?.Invoke();
        }

        public void SetValue(float v)
        {
            value = v;
            onChanged?.Invoke();
        }

        // Called by the owning ModifiedFloat when added / removed.
        internal void Attach(Action recalc)
        {
            onChanged = recalc;
            if (source != null) source.onValueChanged += recalc;
        }

        internal void Detach()
        {
            if (source != null && onChanged != null) source.onValueChanged -= onChanged;
            onChanged = null;
        }
    }

    [SerializeField] private float baseValue;
    [NaughtyAttributes.ReadOnly, SerializeField]
    private Dictionary<string, Modifier> modifiers = new Dictionary<string, Modifier>();

    // Lazy cache so Value is correct even when the object was created by
    // Unity's deserialization (constructor not run). Flag is "calculated"
    // rather than "dirty" so its default (false) means "needs calculating".
    [NonSerialized] private float cached;
    [NonSerialized] private bool calculated;

    public event Action onValueChanged;

    public float Value
    {
        get
        {
            if (!calculated) Recalculate();
            return cached;
        }
    }


    public float BaseValue => baseValue;

    public ModifiedFloat(float baseValue)
    {
        this.baseValue = baseValue;
    }

    private void Recalculate()
    {
        float flatSum = 0f;
        float percentMultiplier = 1f;

        if (modifiers != null)
        {
            foreach (var mod in modifiers.Values)
            {
                if (!mod.Active) continue;
                if (mod.type == ModifierType.Flat) flatSum += mod.Value;
                else percentMultiplier *= mod.Value;
            }
        }

        cached = (baseValue + flatSum) * percentMultiplier;

        calculated = true;
    }

    // Recalculate and notify listeners (including stats that follow this one).
    private void Refresh()
    {
        Recalculate();
        onValueChanged?.Invoke();
    }

    public void SetBaseValue(float v)
    {
        baseValue = v;
        Refresh();
    }

    public Modifier AddModifier(string key, float value, ModifierType type,
        ReplaceModifier replaceRule = ReplaceModifier.None, ModifiedFloat source = null)
    {
        if (modifiers == null) modifiers = new Dictionary<string, Modifier>();

        if (modifiers.TryGetValue(key, out var existing))
        {
            if (replaceRule == ReplaceModifier.IfNewLower && value >= existing.Value) return existing;
            if (replaceRule == ReplaceModifier.IfNewHigher && value <= existing.Value) return existing;
            existing.Detach();
        }

        var mod = new Modifier(value, type, source);
        modifiers[key] = mod;
        mod.Attach(Refresh);
        Refresh();
        return mod;
    }

    public void RemoveModifier(string key)
    {
        if (modifiers == null || !modifiers.TryGetValue(key, out var mod)) return;
        mod.Detach();
        modifiers.Remove(key);
        Refresh();
    }

    public bool HasModifier(string key) => modifiers != null && modifiers.ContainsKey(key);

    public Modifier GetModifier(string key) =>
        modifiers != null && modifiers.TryGetValue(key, out var m) ? m : null;

    public void Reinitialize()
    {
        if (modifiers != null)
            foreach (var mod in modifiers.Values)
            {
                mod.Detach();
                mod.Attach(Refresh);
            }
        Refresh();
    }

}