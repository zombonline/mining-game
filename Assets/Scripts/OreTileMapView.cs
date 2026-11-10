using UnityEngine;
using UnityEngine.Tilemaps;

public class OreTileMapView : MonoBehaviour
{
    [SerializeField] BlockWorld blockWorld;
    [SerializeField] Tilemap oreTileMap;
    private void OnEnable()
    {
        blockWorld.blockDestroyed += HandleBlockDestroyed;
        blockWorld.blocksCreated += HandleBlocksCreated;
    }
    private void OnDisable()
    {
        blockWorld.blockDestroyed -= HandleBlockDestroyed;
        blockWorld.blocksCreated -= HandleBlocksCreated;

    }
    private void HandleBlocksCreated()
    {
        oreTileMap.ClearAllTiles();
        foreach (var blockData in blockWorld.blocks)
        {
            if (blockData.Value.oreType == null)
                continue;
            oreTileMap.SetTile((Vector3Int)blockData.Key, blockData.Value.oreType.tile);
        }
    }
    private void HandleBlockDestroyed(BlockData blockData, Vector2Int pos)
    {
        oreTileMap.SetTile((Vector3Int)pos, null);
    }
}
