using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    public float m_GameDuration = 0f;
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.m_IsGameOver)
        {
            GameManager.m_TotalTime += Time.deltaTime;
        }
        if (GameManager.m_TotalTime > m_GameDuration)
        {
            GameManager.m_IsGameOver = true;
            SceneManager.LoadScene("Stats");

        }

    }
}
