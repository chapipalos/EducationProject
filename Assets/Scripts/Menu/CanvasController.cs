using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject m_MainMenuPanel;
    public GameObject m_LevelSelectorPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_MainMenuPanel.SetActive(true);
        m_LevelSelectorPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        m_MainMenuPanel.SetActive(true);
        m_LevelSelectorPanel.SetActive(false);
    }

    public void ShowLevelSelector()
    {
        m_MainMenuPanel.SetActive(false);
        m_LevelSelectorPanel.SetActive(true);
    }
}
