using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "OreType", menuName = "Scriptable Objects/OreType")]
public class SOOreType : ScriptableObject
{
    public string name;
    public Sprite sprite;
    public Tile tile;
    public int xpValue;
    public Vector2 dropCountRange;
}
