using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Blocks/Block Rule Tile")]
public class BlockRuleTile : RuleTile<BlockRuleTile.Neighbor>
{
    public TileBase[] connectsTo;   // other tiles that count as "this"

    public class Neighbor : RuleTile.TilingRule.Neighbor { }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        if (neighbor == Neighbor.This)
            return tile == this || (tile != null && System.Array.IndexOf(connectsTo, tile) >= 0);

        if (neighbor == Neighbor.NotThis)
            return !(tile == this || (tile != null && System.Array.IndexOf(connectsTo, tile) >= 0));

        return base.RuleMatch(neighbor, tile);
    }
}