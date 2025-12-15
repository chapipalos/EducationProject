using UnityEngine;

public static class GameManager
{
    public static int m_CurrentLevelIndex = 0;

    public static bool m_IsGameOver = false;

    public static float m_TotalTime = 0f;
    public static int m_LevelsCompleted = 0;

    public static int m_RandomPlayer = Random.Range(1, 1000);
    public static string m_PlayerName = $"Player_{m_RandomPlayer}";
    public static bool m_TypeGame;

    public static int m_TotalClicks = 0;
    public static int m_TotalRotations = 0;
}
