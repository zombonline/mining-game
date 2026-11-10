using System;
using UnityEngine;
using static ModifiedFloat;

[System.Serializable]
public class Modifier
{
    public ModifierType type;
    [SerializeField] private float value;
    [SerializeField] private bool active = true;

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

    // called by ModifiedFloat when the modifier is added / removed
    internal void Attach(Action recalc)
    {
        onChanged = recalc;
        if (source != null) source.onValueChanged += recalc;
    }
    internal void Detach()
    {
        if (source != null) source.onValueChanged -= onChanged;
        onChanged = null;
    }
}