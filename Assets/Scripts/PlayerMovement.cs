using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    private Vector2 movementDir;
    private ModifiedFloat moveSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        moveSpeed = SceneContext.Player.Stats.GetStat(PlayerStats.StatKey.MoveSpeed);
    }

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb == null || mouse == null) return;


        Vector2 input = Vector2.zero;
        if (kb.wKey.isPressed) input.y += 1;
        if (kb.sKey.isPressed) input.y -= 1;
        if (kb.dKey.isPressed) input.x += 1;
        if (kb.aKey.isPressed) input.x -= 1;

        movementDir = input.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementDir * moveSpeed.Value * Time.fixedDeltaTime);
    }

}