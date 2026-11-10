using UnityEngine;

public static class GamePause
{
    static int pauseCount;
    public static bool IsPaused => pauseCount > 0;
    public static void Push() { pauseCount++; Time.timeScale = 0f; }
    public static void Pop() { pauseCount = Mathf.Max(0, pauseCount - 1); if (pauseCount == 0) Time.timeScale = 1f; }
}