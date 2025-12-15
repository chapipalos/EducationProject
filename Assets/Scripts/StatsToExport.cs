using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class StatsToExport : MonoBehaviour
{
    [System.Serializable]
    public class GameSummary
    {
        public string playerName;
        public bool typeGame;
        public float totalTime;
        public int totalClicks;
        public int totalRotations;
        public int levelsCompleted;
    }

    [SerializeField] private string webAppUrl = "https://script.google.com/macros/s/AKfycby-eO9hemwqASutvU-AgiIh-JULyGXXrs4Zb_DfEtkPSj7fHqBR2_-wkXuuDewdZTU8/exec";

    public void SendGameSummary()
    {
        GameSummary summary = new GameSummary
        {
            playerName = GameManager.m_PlayerName,
            typeGame = GameManager.m_TypeGame,
            totalTime = GameManager.m_TotalTime,
            totalClicks = GameManager.m_TotalClicks,
            totalRotations = GameManager.m_TotalRotations,
            levelsCompleted = GameManager.m_LevelsCompleted
        };

        StartCoroutine(PostSummary(summary));
    }

    private IEnumerator PostSummary(GameSummary data)
    {
        string json = JsonUtility.ToJson(data);

        UnityWebRequest req = new UnityWebRequest(webAppUrl, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
            Debug.LogError(req.error);
    }
}
