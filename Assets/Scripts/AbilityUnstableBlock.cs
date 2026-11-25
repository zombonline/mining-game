using UnityEngine;

public class AbilityUnstableBlock : MonoBehaviour
{
    private static readonly Vector2Int[] cardinalDirections =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private void OnEnable()
    {
        SceneContext.BlockWorld.blockDestroyed += HandleBlockDestroyed;
    }

    private void OnDisable()
    {
        SceneContext.BlockWorld.blockDestroyed -= HandleBlockDestroyed;
    }

    private void HandleBlockDestroyed(BlockData blockData,Vector2Int blockPos)
    {
        if (!blockData.unstable)
            return;
        foreach (Vector2Int dir in cardinalDirections)
        {
            for (int distance = 1; distance <= 3; distance++)
            {
                Vector2Int targetPos = blockPos + (dir * distance);
                SceneContext.BlockWorld.Hit(targetPos, 8);
            }
        }
    }
}
