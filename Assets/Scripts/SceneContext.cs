using UnityEngine;

public class SceneContext : MonoBehaviour
{
    [SerializeField] private Player player;
    public static Player Player;
    [SerializeField] private LevelManager levelManager;
    public static LevelManager LevelManager;
    [SerializeField] private UpgradesManager upgradesManager;
    public static UpgradesManager UpgradesManager;
    [SerializeField] private BlockWorld blockWorld;
    public static BlockWorld BlockWorld;

    private void Awake()
    {
        Player = player;
        LevelManager = levelManager;
        UpgradesManager = upgradesManager;
        BlockWorld = blockWorld;
    }
}
