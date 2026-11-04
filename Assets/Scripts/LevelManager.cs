using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public  SOLevelSettings levelSettings;


    //THIS SCRIPT IS PURELY FOR RUNNING THE LEVEL, I.E MOVING THE BARRIER HEIGHT VALUE UP, CHECKING PLAYER POSITION RELATIVE TO THE BARRIER
    public float barrierY;
    public float highestPlayerY;
    public GameObject player;


    private void Update()
    {
        barrierY += GetSpeed(highestPlayerY) * Time.deltaTime;
        highestPlayerY = Mathf.Max(player.transform.position.y, highestPlayerY);
    }
    private void LateUpdate()
    {
        if (player.transform.position.y < barrierY)
            Debug.Log("You lose");
    }

    private void OnDrawGizmos()
    {
        var cameraLeft = Camera.main.ScreenToWorldPoint(new Vector3(0,0,0)).x;
        var cameraRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0)).x;
        Gizmos.DrawLine(new Vector2(cameraLeft, barrierY), new Vector2(cameraRight, barrierY));
    }
    public float GetSpeed(float depth)
        => levelSettings.baseSpeed + levelSettings.growth * Mathf.Log(1f + Mathf.Max(0f, depth) / levelSettings.scale);
}
