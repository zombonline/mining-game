using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShockwave : MonoBehaviour
{
    private float lastTime = 0f;
    public float secondsBetween = 6f; //TO BE MADE INTO A STAT
    public float damage = 1f;
    public float radius = 1f;

    private static readonly Vector2Int[] cardinalDirections =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    private void Update()
    {
        if (Time.time < lastTime + secondsBetween)
            return;
        lastTime = Time.time;
        var squareRadius = Mathf.Floor(radius / 2);
        Vector2Int playerPos = Vector2Int.RoundToInt(SceneContext.Player.transform.position);
        List<Vector2Int> hitPositions = new List<Vector2Int>();
        foreach (Vector2Int dir in cardinalDirections)
        {
            for (int distance = 1; distance <= radius; distance++)
            {
                Vector2Int targetPos = playerPos + (dir * distance);
                SceneContext.BlockWorld.Hit(targetPos, 8);
                hitPositions.Add(targetPos);
            }
        }

    }
}
