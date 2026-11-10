using UnityEngine;

[System.Serializable]
public struct NoisePass
{
    public float xScale, yScale;
    public Vector2[] thresholds;
}
[System.Serializable]
public struct BlockNoisePass
{
    public NoisePass noisePass;
    public SOBlockType blockType;
    [Header("Over depth")]
    public float minY;                 // pass does nothing below this
    public float fadeIn;               // blocks over which presence ramps 0 -> 1 (0 = instant)
    public float waveLength;           // 1D noise wavelength in blocks (0 = no wave)
    [Range(0f, 1f)] public float waveStrength; // 0 = steady, 1 = can fully vanish

    public float Presence(float y, float waveOffset)
    {
        if (y < minY) return 0f;

        float ramp = fadeIn <= 0f ? 1f : Mathf.SmoothStep(0f, 1f, (y - minY) / fadeIn);

        float wave = 1f;
        if (waveLength > 0f)
        {
            float raw = Mathf.PerlinNoise(y / waveLength + waveOffset, 0.5f);
            wave = Mathf.InverseLerp(0.25f, 0.75f, raw);   // Perlin hugs 0.5, so stretch it
            wave = Mathf.Lerp(1f - waveStrength, 1f, wave);
        }
        return ramp * wave;
    }

    public bool Hits(float noise, float presence)
    {
        if (presence <= 0f) return false;
        foreach (var t in noisePass.thresholds)
        {
            float centre = (t.x + t.y) * 0.5f;
            float half = (t.y - t.x) * 0.5f * presence;
            if (Mathf.Abs(noise - centre) <= half) return true;
        }
        return false;
    }

}
[System.Serializable]
public struct OrePass
{
    public SOOreType oreType;
    [Range(0f, 1f)] public float chance;      // per eligible block, at full presence
    public float minY, fadeIn;
    public SOBlockType[] hostBlocks;          // empty = any block

    [Header("Clumping (optional)")]
    public float clusterScale;                // 0 = pure scatter
    [Range(0f, 1f)] public float clusterThreshold; // only roll inside noise >= this

    public bool CanHost(SOBlockType b)
    {
        if (hostBlocks == null || hostBlocks.Length == 0) return true;
        return System.Array.IndexOf(hostBlocks, b) >= 0;
    }

    public float Chance(int y)
    {
        if (y < minY) return 0f;
        float ramp = fadeIn <= 0f ? 1f : Mathf.SmoothStep(0f, 1f, (y - minY) / fadeIn);
        return chance * ramp;
    }
}
[System.Serializable]
[CreateAssetMenu(fileName = "CaveGenParameters", menuName = "Scriptable Objects/CaveGenParameters")]
public class SOCaveGenParameters : ScriptableObject
{
    public BlockNoisePass[] blockNoisePasses;
    public OrePass[] orePasses;
}
