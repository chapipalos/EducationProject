using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
    public List<Mesh> m_Levels = new List<Mesh>();
    public List<Button> m_Buttons = new List<Button>();

    public int m_FirstLevelIndex = 0;

    public Button m_PreviousButton;
    public Button m_NextButton;

    void Start()
    {
        SetButtons();
        m_PreviousButton.onClick.AddListener(OnPreviousButtonClicked);
        m_NextButton.onClick.AddListener(OnNextButtonClicked);
        foreach (Button button in m_Buttons)
        {
            button.onClick.AddListener(OnLevelButtonClicked);
        }
    }

    private void SetButtons()
    {
        for (int i = 0; i < m_Buttons.Count; i++)
        {
            int levelIndex = (m_FirstLevelIndex + i) % m_Levels.Count;

            m_Buttons[i].GetComponentInChildren<MeshFilter>().mesh = m_Levels[levelIndex];
            m_Buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = m_Levels[levelIndex].name;
        }
    }

    private void OnPreviousButtonClicked()
    {
        m_FirstLevelIndex = (m_FirstLevelIndex - 1 + m_Levels.Count) % m_Levels.Count;
        SetButtons();
    }

    private void OnNextButtonClicked()
    {
        m_FirstLevelIndex = (m_FirstLevelIndex + 1) % m_Levels.Count;
        SetButtons();
    }

    private void OnLevelButtonClicked()
    {
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        int buttonIndex = m_Buttons.IndexOf(clickedButton);
        int levelIndex = (m_FirstLevelIndex + buttonIndex) % m_Levels.Count;
        GameManager.m_CurrentLevelIndex = levelIndex;
        Debug.Log("Selected Level: " + m_Levels[levelIndex].name);
    }
}
