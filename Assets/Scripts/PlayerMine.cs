using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMine : MonoBehaviour
{
    [SerializeField] BlockWorld blockWorld;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] LayerMask blockSortingLayer;
    [SerializeField] Tilemap blockTilemap;
    [SerializeField] Transform aimTransform;
    private float lastMineTime = 0f;

    [SerializeField] private float mineReach = 3f;
    public struct MineTarget
    {
        public Vector2Int cell;
        public bool isPrimary;
    }

    [SerializeField] float cellSize = 1f;

    Vector2Int WorldToCell(Vector2 p) =>
        new Vector2Int(Mathf.FloorToInt(p.x / cellSize), Mathf.FloorToInt(p.y / cellSize));

    Vector2 CellToWorld(Vector2Int c) =>
        new Vector2((c.x + 0.5f) * cellSize, (c.y + 0.5f) * cellSize);

    private void Update()
    {
        if (Keyboard.current.spaceKey.isPressed && (Time.time - lastMineTime) >= cooldown)
        {
            Mine();
            lastMineTime = Time.time;
        }
    }

    public void Mine()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimTransform.up, mineReach, blockSortingLayer);
        if(hit)
        {
            Vector3Int tilePos = blockTilemap.WorldToCell(new Vector3(hit.point.x - (hit.normal.x * .01f), hit.point.y - (hit.normal.y * .01f), 0));
            blockWorld.Hit((Vector2Int)tilePos, 1);
        }
    }
}
