using UnityEngine;

public class XpOrb : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] float pullRadius = 3f;
    [SerializeField] float collectRadius = 0.3f;
    [SerializeField] float startSpeed = 2f;
    [SerializeField] float acceleration = 40f;
    [SerializeField] float maxSpeed = 25f;

    [Header("Burst")]
    [SerializeField] Vector2 burstDistanceRange = new Vector2(0.5f, 1.2f);
    [SerializeField] Vector2 burstDurationRange = new Vector2(0.25f, 0.4f);

    enum State { Bursting, Idle, Attracted }
    State state;

    Vector2 burstStart, burstTarget;
    float burstTime, burstDuration;
    float speed;

    private int value = 1;

    // Call from the spawner right after Instantiate
    public void Launch(Vector2 origin)
    {
        Debug.Log("Launching from: " + origin);
        transform.position = origin;
        burstStart = origin;
        burstTarget = origin + Random.insideUnitCircle.normalized * Random.Range(burstDistanceRange.x, burstDistanceRange.y);
        burstDuration = Random.Range(burstDurationRange.x, burstDurationRange.y);
        burstTime = 0f;
        state = State.Bursting;
    }

    void Update()
    {
        switch (state)
        {
            case State.Bursting: UpdateBurst(); break;
            case State.Idle: UpdateIdle(); break;
            case State.Attracted: UpdateAttracted(); break;
        }
    }

    void UpdateBurst()
    {
        burstTime += Time.deltaTime;
        float t = Mathf.Clamp01(burstTime / burstDuration);
        float eased = 1f - (1f - t) * (1f - t) * (1f - t); // ease-out cubic
        transform.position = Vector2.LerpUnclamped(burstStart, burstTarget, eased);

        if (t >= 1f) state = State.Idle;
    }

    void UpdateIdle()
    {
        float dist = ((Vector2)(Player.Instance.transform.position - transform.position)).magnitude;
        if (dist <= pullRadius)
        {
            speed = startSpeed;
            state = State.Attracted;
        }
    }

    void UpdateAttracted()
    {
        Vector2 toPlayer = Player.Instance.transform.position - transform.position;
        float dist = toPlayer.magnitude;

        speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxSpeed);
        transform.position += (Vector3)(toPlayer / Mathf.Max(dist, 0.0001f) * speed * Time.deltaTime);

        if (dist <= collectRadius) Collect();
    }

    void Collect()
    {
        Player.Instance.XP.AddXp(value);
        Destroy(gameObject);
    }
}