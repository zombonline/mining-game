using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] Transform aimTransform;
    [SerializeField] float turnSpeed = 720f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 diff = mouseWorld - (Vector2)aimTransform.position;

        if (diff.sqrMagnitude < 0.01f) return;

        float targetAngle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.MoveTowardsAngle(aimTransform.eulerAngles.z, targetAngle, turnSpeed * Time.deltaTime);
        aimTransform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}