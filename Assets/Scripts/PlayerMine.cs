using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static ModifiedFloat;
using static PlayerStats;

public class PlayerMine : MonoBehaviour
{
    enum SwingState { Idle, Swinging, Cooldown }

    [Header("Swing")]

    [SerializeField] float swingSpeed = 400f;      // degrees per second
    [SerializeField] float maxStepDegrees = 3f;    // finer = fewer missed blocks
    [SerializeField] bool pierce = true;           // hit every block on the line, or just the first
    [SerializeField] float originWidth = 0.2f;


    [Header("References")]
    [SerializeField] LayerMask blockSortingLayer;
    [SerializeField] Tilemap blockTilemap;
    [SerializeField] Transform aimTransform;
    [SerializeField] BlockWorld blockWorld;

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

    private bool held = false;
    public event Action<bool> heldStateChanged;
    [SerializeField] Transform pickaxeHandle;

    void Awake()
    {
        filter = new ContactFilter2D { useLayerMask = true, layerMask = blockSortingLayer, useTriggers = false };
    }
    private void Start()
    {
        var penalty = SceneContext.Player.Stats.GetStat(StatKey.MoveSpeed)
    .AddModifier("Mining", 1f, ModifierType.Percent, source: SceneContext.Player.Stats.GetStat(StatKey.MoveSpeedMining));
        penalty.SetActive(false);
        heldStateChanged += penalty.SetActive;
    }

    void Update()
    {
        var heldThisFrame = Mouse.current.leftButton.isPressed;
        if (held != heldThisFrame)
            heldStateChanged?.Invoke(heldThisFrame);
        held = heldThisFrame;

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
        swingAngle = -SceneContext.Player.Stats.GetStat(StatKey.PickaxeSwingArc).Value * 0.5f * swingDir;  
        hitThisPass.Clear();
        CastAt(swingAngle);
    }

    void AdvanceSwing(float dt)
    {
        float half = SceneContext.Player.Stats.GetStat(StatKey.PickaxeSwingArc).Value * 0.5f;
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
                cooldownEndTime = Time.time + SceneContext.Player.Stats.GetStat(StatKey.PickaxeCooldown).Value;
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
        pickaxeHandle.transform.position = origin;
        float angleDegrees = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        pickaxeHandle.transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees - 90);
        int count = Physics2D.Raycast(origin, dir, filter, hitBuffer, SceneContext.Player.Stats.GetStat(StatKey.PickaxeReach).Value);
        for (int i = 0; i < count; i++)
        {
            var hit = hitBuffer[i];
            Vector3 inside = hit.point - hit.normal * 0.01f;
            var cell = (Vector2Int)blockTilemap.WorldToCell(inside);

            if (hitThisPass.Add(cell))
            {
                blockWorld.Hit(cell, SceneContext.Player.Stats.GetStat(StatKey.PickaxeDamage).IntValue);
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
        float half = SceneContext.Player.Stats.GetStat(StatKey.PickaxeSwingArc).Value * 0.5f;

        // Arc outline
        Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
        Vector3 prev = origin + (Vector3)(DirFromOffset(-half) * SceneContext.Player.Stats.GetStat(StatKey.PickaxeReach).Value);
        Vector3 originLeft = origin + (aimTransform.right * -originWidth/2);
        Vector3 originRight = origin + (aimTransform.right * originWidth/2);
        Gizmos.DrawLine(originLeft, originRight);
        Gizmos.DrawLine(originRight, prev);

        const int segments = 24;
        for (int i = 1; i <= segments; i++)
        {
            float a = Mathf.Lerp(-half, half, i / (float)segments);
            Vector3 next = origin + (Vector3)(DirFromOffset(a) * SceneContext.Player.Stats.GetStat(StatKey.PickaxeReach).Value);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
        Gizmos.DrawLine(originLeft, prev);

        // Aim centre line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + aimTransform.up * SceneContext.Player.Stats.GetStat(StatKey.PickaxeReach).Value);

        if (Application.isPlaying)
        {
            // Swing ray: red while swinging, yellow while resting in cooldown
            if (state != SwingState.Idle)
            {
                Vector2 modifiedOrigin = origin + (aimTransform.right * originOffset);
                Vector3 tip = origin + (Vector3)(DirFromOffset(swingAngle) * SceneContext.Player.Stats.GetStat(StatKey.PickaxeReach).Value);
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