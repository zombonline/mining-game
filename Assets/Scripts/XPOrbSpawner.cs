using UnityEngine;

public class XPOrbSpawner : MonoBehaviour
{
    [SerializeField] XpOrb XPOrbPrefab;
    [SerializeField] SOOreType blockDebris;
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
            var newXp = Instantiate(XPOrbPrefab, transform);
            newXp.SetXPType(blockDebris.sprite, blockDebris.xpValue);
            newXp.Launch(blockPos);
        }
        if(blockData.oreType!=null)
        {
            var oreType = blockData.oreType;
            for (int i = 0; i < Random.Range(oreType.dropCountRange.x, oreType.dropCountRange.y); i++)
            {
                var newXp = Instantiate(XPOrbPrefab, transform);
                newXp.SetXPType(oreType.sprite, oreType.xpValue);
                newXp.Launch(blockPos);
            }
        }

    }
}
