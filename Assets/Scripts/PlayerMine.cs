using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerMine : MonoBehaviour
{
    enum SwingState { Idle, Swinging, Cooldown }

    [Header("Swing")]
    [SerializeField] float mineReach = 1.5f;
    [SerializeField] float arcDegrees = 100f;      // total sweep width
    [SerializeField] float swingSpeed = 400f;      // degrees per second
    [SerializeField] float swingCooldown = 0.3f;   // pause after reaching the far side
    [SerializeField] float maxStepDegrees = 3f;    // finer = fewer missed blocks
    [SerializeField] bool pierce = true;           // hit every block on the line, or just the first
    [SerializeField] float originWidth = 0.2f;
    [SerializeField] int hitPoints = 1;

    [Header("References")]
    [SerializeField] LayerMask blockSortingLayer;
    [SerializeField] Tilemap blockTilemap;
    [SerializeField] Transform aimTransform;
    [SerializeField] BlockWorld blockWorld;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Gizmos")]
    [SerializeField] bool drawGizmos = true;
    [SerializeField] float hitFlashDuration = 0.2f;

    SwingState state = SwingState.Idle;
    float swingAngle;           // current offset from aim, in degrees
    float originOffset;
    int swingDir = 1;           // 1 = sweeping +, -1 = sweeping back
    float cooldownEndTime;

    readonly HashSet<Vector2Int> hitThisPass = new HashSet<Vector2Int>();
    readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[8];
    ContactFilter2D filter;

    struct HitFlash { public Vector3 pos; public float time; }
    readonly List<HitFlash> recentHits = new List<HitFlash>();


    void Awake()
    {
        filter = new ContactFilter2D { useLayerMask = true, layerMask = blockSortingLayer, useTriggers = false };
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        var held = Mouse.current.leftButton.isPressed;
        if (held && !playerMovement.speed.HasModifier("mining"))
            playerMovement.speed.AddModifier("mining", 0.5f, ModifiedFloat.ModifierType.Percent);
        else if(!held)
            playerMovement.speed.RemoveModifier("mining");


            switch (state)
            {
                case SwingState.Idle:
                    if (held) BeginSwing();
                    break;

                case SwingState.Swinging:
                    AdvanceSwing(Time.deltaTime);   // ignores the button: swing always completes
                    break;

                case SwingState.Cooldown:
                    if (Time.time >= cooldownEndTime)
                    {
                        state = SwingState.Idle;
                        if (held) BeginSwing();     // still holding: next swing starts from this side
                    }
                    break;
            }
    }

    void BeginSwing()
    {
        state = SwingState.Swinging;
        swingAngle = -arcDegrees * 0.5f * swingDir;  // start at the side we finished on
        hitThisPass.Clear();
        CastAt(swingAngle);
    }

    void AdvanceSwing(float dt)
    {
        float half = arcDegrees * 0.5f;
        float remaining = swingSpeed * dt;


        while (remaining > 0f)
        {
            float step = Mathf.Min(remaining, maxStepDegrees);
            remaining -= step;

            swingAngle += step * swingDir;
            var swingPercent = swingAngle / half;
            originOffset = swingPercent * -originWidth/2;

            bool reachedEnd = Mathf.Abs(swingAngle) >= half;
            if (reachedEnd) swingAngle = half * swingDir;   // clamp so we never overshoot the arc



            CastAt(swingAngle);

            if (reachedEnd)
            {
                swingDir = -swingDir;                        // next swing goes the other way
                state = SwingState.Cooldown;
                cooldownEndTime = Time.time + swingCooldown;
                return;
            }
        }
    }

    Vector2 DirFromOffset(float angleOffset) =>
        Quaternion.AngleAxis(angleOffset, Vector3.forward) * aimTransform.up;

    void CastAt(float angleOffset)
    {
        Vector2 dir = DirFromOffset(angleOffset);
        Vector2 origin = transform.position + (aimTransform.right * originOffset);

        int count = Physics2D.Raycast(origin, dir, filter, hitBuffer, mineReach);
        for (int i = 0; i < count; i++)
        {
            var hit = hitBuffer[i];
            Vector3 inside = hit.point - hit.normal * 0.01f;
            var cell = (Vector2Int)blockTilemap.WorldToCell(inside);

            if (hitThisPass.Add(cell))
            {
                blockWorld.Hit(cell, hitPoints);
#if UNITY_EDITOR
                recentHits.RemoveAll(h => Time.time - h.time > hitFlashDuration);
                recentHits.Add(new HitFlash
                {
                    pos = blockTilemap.GetCellCenterWorld((Vector3Int)cell),
                    time = Time.time
                });
#endif
            }

            if (!pierce) break;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!drawGizmos || aimTransform == null) return;

        Vector3 origin = transform.position;
        float half = arcDegrees * 0.5f;

        // Arc outline
        Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
        Vector3 prev = origin + (Vector3)(DirFromOffset(-half) * mineReach);
        Vector3 originLeft = origin + (aimTransform.right * -originWidth/2);
        Vector3 originRight = origin + (aimTransform.right * originWidth/2);
        Gizmos.DrawLine(originLeft, originRight);
        Gizmos.DrawLine(originRight, prev);

        const int segments = 24;
        for (int i = 1; i <= segments; i++)
        {
            float a = Mathf.Lerp(-half, half, i / (float)segments);
            Vector3 next = origin + (Vector3)(DirFromOffset(a) * mineReach);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
        Gizmos.DrawLine(originLeft, prev);

        // Aim centre line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + aimTransform.up * mineReach);

        if (Application.isPlaying)
        {
            // Swing ray: red while swinging, yellow while resting in cooldown
            if (state != SwingState.Idle)
            {
                Vector2 modifiedOrigin = origin + (aimTransform.right * originOffset);
                Vector3 tip = origin + (Vector3)(DirFromOffset(swingAngle) * mineReach);
                Gizmos.color = state == SwingState.Swinging ? Color.red : Color.yellow;
                Gizmos.DrawLine(modifiedOrigin, tip);
                Gizmos.DrawWireSphere(tip, 0.05f);
            }

            // Recently hit cells, fading out
            foreach (var h in recentHits)
            {
                float t = (Time.time - h.time) / hitFlashDuration;
                if (t > 1f) continue;
                Gizmos.color = new Color(1f, 0.6f, 0f, 1f - t);
                Gizmos.DrawWireCube(h.pos, blockTilemap.cellSize * 0.95f);
            }
        }
    }
#endif
}