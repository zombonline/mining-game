using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "Scriptable Objects/LevelSettings")]
public class SOLevelSettings : ScriptableObject
{
    public float levelWidth, levelHeight;
    public float baseSpeed = 1.0f;
    public float growth = 0.8f;
    public float scale = 100f;
    public SOCaveGenParameters caveGenParameters;
}
