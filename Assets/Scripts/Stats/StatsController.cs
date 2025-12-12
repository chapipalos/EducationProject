using TMPro;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public TextMeshProUGUI m_LevelsScoreText;
    public TextMeshProUGUI m_TimeScoreText;

    void Start()
    {
        m_LevelsScoreText.text = GameManager.m_LevelsCompleted.ToString();
        m_TimeScoreText.text = GameManager.m_TotalTime.ToString() + "s";
    }
}
