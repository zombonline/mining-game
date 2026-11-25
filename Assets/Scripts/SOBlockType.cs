using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BlockType", menuName = "Scriptable Objects/BlockType")]
public class SOBlockType : ScriptableObject
{
    public string name;
    public float health;
    [ShowAssetPreview]
    public Sprite sprite;
    [ShowAssetPreview]
    public TileBase tile, unstableTile;
    public int xp;
}
