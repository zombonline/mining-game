using UnityEngine;

[System.Serializable]
public struct NoisePass
{
    public float xScale, yScale;
    public Vector2[] thresholds;
}
[CreateAssetMenu(fileName = "CaveGenParameters", menuName = "Scriptable Objects/CaveGenParameters")]
public class SOCaveGenParameters : ScriptableObject
{
    public NoisePass[] noisePasses;
}
