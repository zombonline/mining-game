using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class SOBlockType : ScriptableObject
{
    public string name;
    public int health;
    [ShowAssetPreview]
    public Sprite sprite;
    [ShowAssetPreview]
    public TileBase tile;
    public int xp;
}
