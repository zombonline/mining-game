using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public PlayerXp XP;
    public PlayerMine Mine;
    public PlayerMovement Movement;
    public PlayerAim Aim;
    void Awake()
    {
        XP = GetComponent<PlayerXp>();
        Mine = GetComponent<PlayerMine>();
        Movement = GetComponent<PlayerMovement>();
        Aim = GetComponent<PlayerAim>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}