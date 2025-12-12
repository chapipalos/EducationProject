using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.m_IsGameOver)
        {
            GameManager.m_TotalTime += Time.deltaTime;
        }
        if (GameManager.m_TotalTime > 35f)
        {
            GameManager.m_IsGameOver = true;
            SceneManager.LoadScene("Stats");
        }
    }
}
