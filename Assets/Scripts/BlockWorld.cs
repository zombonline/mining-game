using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockData
{
    public SOBlockType blockType;
    public int damage;
    public BlockData(SOBlockType type)
    {
        this.blockType = type;
        this.damage = 0;
    }
}

[System.Serializable]
public struct BlockSqaure
{
    public SOBlockType[] types;
    public int x;
    public int y;
    public float noiseScale;
}

public class BlockWorld : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector2Int, BlockData> blocks = new Dictionary<Vector2Int, BlockData>();
    [SerializeField] private SOLevelSettings levelSettings;
    [SerializeField] BlockSqaure blockSqaure;

    private void Awake()
    {
        blocks.Clear();
        for (int x = 0; x < levelSettings.levelWidth; x++)
        {
            for (int y = 0; y < levelSettings.levelHeight; y++)
            {
                bool isCave = false;
                foreach (var noisePass in levelSettings.caveGenParameters.noisePasses)
                {
                    var pathNoiseValue = Mathf.PerlinNoise(x / noisePass.xScale, y / noisePass.yScale);
                    if (pathNoiseValue > noisePass.lowerThreshold && pathNoiseValue < noisePass.upperThreshold)
                        isCave = true;
                }
                if (isCave)
                    continue;
                var noiseValue = Mathf.PerlinNoise(x / blockSqaure.noiseScale, y / blockSqaure.noiseScale);
                SOBlockType type;
                if (noiseValue > 0.5)
                    type = blockSqaure.types[0];
                else
                    type = blockSqaure.types[1];
                blocks.Add(new Vector2Int(x, y), new BlockData(type));
            }
        }
    }

    [Button]
    public void GenerateBlockSqaure()
    {
        blocks.Clear();
        for(int i = 0; i < blockSqaure.x; i++)
        {
            for(int j = 0; j < blockSqaure.y; j++)
            { 
                var noiseValue = Mathf.PerlinNoise(i / blockSqaure.noiseScale, j / blockSqaure.noiseScale);
                SOBlockType type;
                if (noiseValue > 0.5)
                    type = blockSqaure.types[0];
                else
                    type = blockSqaure.types[1];
                Debug.Log("Noise value: " + noiseValue);
                blocks.Add(new Vector2Int(i, j),new BlockData(type));
            }
        }
        Debug.Log("Placed " + blockSqaure.x * blockSqaure.y + " blocks");
        blocksCreated.Invoke();
    }

    public void Hit(Vector2Int blockPos, int hitPoints)
    {
        var blockData = blocks[blockPos];
        blockData.damage += hitPoints;
        blocks[blockPos] = blockData;
        Debug.Log("Hit tile at " + blockPos + ", damage now " + blockData.damage);
        blockDamaged?.Invoke(blockData, blockPos);
        if (blockData.damage >= blockData.blockType.health)
        {
            blocks.Remove(blockPos);
            blockDestroyed?.Invoke(blockData, blockPos);
        }
    }

    public event Action<BlockData, Vector2Int> blockDestroyed, blockDamaged;
    public event Action blocksCreated;
}
