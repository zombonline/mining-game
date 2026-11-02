using UnityEngine;
using UnityEngine.Tilemaps;

public class DamageTileMapView : MonoBehaviour
{
    [SerializeField] BlockWorld blockWorld;
    [SerializeField] Tilemap damageTilemap;
    [SerializeField] Tile[] damageStateTiles;
    private void Start()
    {
    }
    private void OnEnable()
    {
        blockWorld.blockDestroyed += HandleBlockDestroyed; 
        blockWorld.blockDamaged += HandeBlockDamaged;
    }
    private void OnDisable()
    {
        blockWorld.blockDestroyed += HandleBlockDestroyed;
        blockWorld.blockDamaged += HandeBlockDamaged;
    }

    private void HandeBlockDamaged(BlockData blockData, Vector2Int pos)
    {
        if (blockData.damage >= blockData.blockType.health)
            return;
        float healthPercentage = (float)blockData.damage / blockData.blockType.health;
        int index = Mathf.FloorToInt(damageStateTiles.Length * healthPercentage);
        Debug.Log("Index: " + index +" health percent " + healthPercentage);

        var damageStateTile = damageStateTiles[index];
        damageTilemap.SetTile((Vector3Int)pos, damageStateTile);
    }
    private void HandleBlockDestroyed(BlockData blockData, Vector2Int pos)
    {
        damageTilemap.SetTile((Vector3Int)pos, null);
    }
}
