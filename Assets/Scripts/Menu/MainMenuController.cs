using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public TMP_InputField m_PlayerNameInput;

    public Button m_EnterPlayerNameButton;

    public Toggle m_TypeGameToggle;

    public CanvasController m_CanvasController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_EnterPlayerNameButton.onClick.AddListener(EnterPlayerName);
    }

    private void EnterPlayerName()
    {
        if(string.IsNullOrEmpty(m_PlayerNameInput.text) && string.IsNullOrWhiteSpace(m_PlayerNameInput.text))
        {
            Debug.LogWarning("El nombre del jugador no puede estar vacío.");
            return;
        }
        GameManager.m_PlayerName = m_PlayerNameInput.text;
        Debug.Log("Nombre del jugador establecido a: " + GameManager.m_PlayerName);

        GameManager.m_TypeGame = m_TypeGameToggle.isOn;

        GameManager.m_CurrentLevelIndex = 0;
        SceneManager.LoadScene("Game");
    }
}
