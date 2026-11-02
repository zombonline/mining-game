using UnityEngine;

[System.Serializable]
public struct NoisePass
{
    public float xScale, yScale;
    public float lowerThreshold;
    public float upperThreshold;
}
[CreateAssetMenu(fileName = "CaveGenParameters", menuName = "Scriptable Objects/CaveGenParameters")]
public class SOCaveGenParameters : ScriptableObject
{
    public NoisePass[] noisePasses;
}
