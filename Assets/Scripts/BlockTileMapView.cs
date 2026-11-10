using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockTileMapView : MonoBehaviour
{
    [SerializeField] BlockWorld blockWorld;
    [SerializeField] Tilemap blockTilemap;
    private void Start()
    {
        HandleBlocksCreated();
    }
    private void OnEnable()
    {
        blockWorld.blocksCreated += HandleBlocksCreated;
        blockWorld.blockDestroyed += HandleBlockDestroyed;
    }
    private void OnDisable()
    {
        blockWorld.blocksCreated -= HandleBlocksCreated;
        blockWorld.blockDestroyed -= HandleBlockDestroyed;
    }

    private void HandleBlocksCreated()
    {
        blockTilemap.ClearAllTiles();
        foreach(var blockData in blockWorld.blocks)
        {
            blockTilemap.SetTile((Vector3Int)blockData.Key, blockData.Value.blockType.tile);
        }
    }
    private void HandleBlockDestroyed(BlockData data, Vector2Int pos)
    {
        blockTilemap.SetTile((Vector3Int)pos, null);
    }
}
