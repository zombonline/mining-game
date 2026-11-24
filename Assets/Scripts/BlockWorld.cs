using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BlockData
{
    public SOBlockType blockType;
    public int damage;
    public SOOreType oreType;
    public BlockData(SOBlockType type, SOOreType oreType)
    {
        this.blockType = type;
        this.damage = 0;
        this.oreType = oreType;
    }
}



public class BlockWorld : MonoBehaviour
{
    [SerializeField]
    public Dictionary<Vector2Int, BlockData> blocks = new Dictionary<Vector2Int, BlockData>();
    [SerializeField] private SOLevelSettings levelSettings;
    [SerializeField] SOBlockType baseBlock;
    private int seed = 102450; //temp seed

    private void Start()
    {
        blocks.Clear();
        var passes = levelSettings.caveGenParameters.blockNoisePasses;
        var rng = new System.Random(seed);
        var offsets = new Vector2[passes.Length];
        var waveOffsets = new float[passes.Length];
        
        for (int i = 0; i < passes.Length; i++)
        {
            offsets[i] = new Vector2(rng.Next(0, 10000), rng.Next(0, 10000));
            waveOffsets[i] = rng.Next(0, 10000);
        }
        var orePasses = levelSettings.caveGenParameters.orePasses;
        var oreOffsets = new Vector2[orePasses.Length];
        for (int i = 0; i < orePasses.Length; i++)
            oreOffsets[i] = new Vector2(rng.Next(0, 10000), rng.Next(0, 10000));
        for (int x = 0; x < levelSettings.levelWidth; x++)
        {
            var shiftedX = Mathf.FloorToInt(x - levelSettings.levelWidth / 2);

            for (int y = 0; y < levelSettings.levelHeight; y++)
            {
                var blockPos = new Vector2Int(shiftedX, y);
                SOBlockType finalBlockType = baseBlock;
                for (int i = 0; i < passes.Length; i++)
                {
                    var p = passes[i];
                    float presence = p.Presence(y, waveOffsets[i]);
                    if (presence <= 0f) continue;

                    float n = Mathf.PerlinNoise(
                        shiftedX / p.noisePass.xScale + offsets[i].x,
                        y / p.noisePass.yScale + offsets[i].y);

                    if (p.Hits(n, presence))
                        finalBlockType = p.blockType;
                }
                if (finalBlockType != null)
                {
                    SOOreType ore = null;
                    for (int o = 0; o < orePasses.Length; o++)       // list rare ores first
                    {
                        var op = orePasses[o];
                        if (!op.CanHost(finalBlockType)) continue;
                        float c = op.Chance(y);
                        if (c <= 0f) continue;

                        if (op.clusterScale > 0f)
                        {
                            float cn = Mathf.PerlinNoise(
                                shiftedX / op.clusterScale + oreOffsets[o].x,
                                y / op.clusterScale + oreOffsets[o].y);
                            if (cn < op.clusterThreshold) continue;
                        }

                        if (Hash01(shiftedX, y, seed + o * 7919) < c) { ore = op.oreType; break; }
                    }
                    SetBlock(blockPos, finalBlockType, ore);
                }
            }
        }
        blocksCreated?.Invoke();
    }

    private void SetBlock(Vector2Int blockPos, SOBlockType blockType, SOOreType oreType = null)
    {
        if (blocks.ContainsKey(blockPos))
            blocks[blockPos] = new BlockData(blockType, oreType);
        else
            blocks.Add(blockPos, new BlockData(blockType, oreType));
    }


    public void Hit(Vector2Int blockPos, int hitPoints)
    {
        if (!blocks.TryGetValue(blockPos, out var blockData) || blockData.blockType == null || blockData.blockType.health == -1) 
            return;
        blockData.damage += hitPoints * SceneContext.Player.Stats.GetStat(PlayerStats.StatKey.DamageBase).IntValue;
        blocks[blockPos] = blockData;
        blockDamaged?.Invoke(blockData, blockPos);
        if (blockData.damage >= blockData.blockType.health)
        {
            blocks.Remove(blockPos);
            blockDestroyed?.Invoke(blockData, blockPos);

        }
    }
    static float Hash01(int x, int y, int seed)
    {
        unchecked
        {
            uint h = (uint)seed;
            h ^= (uint)x * 0x27d4eb2du; h = (h ^ (h >> 15)) * 0x85ebca6bu;
            h ^= (uint)y * 0x165667b1u; h = (h ^ (h >> 13)) * 0xc2b2ae35u;
            h ^= h >> 16;
            return (h & 0xFFFFFF) / (float)0x1000000;   // 0..1
        }
    }

    public event Action<BlockData, Vector2Int> blockDestroyed, blockDamaged;
    public event Action blocksCreated;
}
