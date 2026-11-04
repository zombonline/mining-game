using UnityEngine;

public class XPOrbSpawner : MonoBehaviour
{
    [SerializeField] XpOrb whiteXp;
    [SerializeField] BlockWorld blockWorld;

    private void OnEnable()
    {
        blockWorld.blockDestroyed += HandleBlockDestroyed;
    }

    private void OnDisable()
    {
        blockWorld.blockDestroyed -= HandleBlockDestroyed;
    }

    private void HandleBlockDestroyed(BlockData blockData, Vector2Int blockPos)
    {
        for (int i = 0; i < blockData.blockType.xp; i++)
        {
            var newXp = Instantiate(whiteXp, transform);
            newXp.Launch(blockPos);
        }

    }
}
