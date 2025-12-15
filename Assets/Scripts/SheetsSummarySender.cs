using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class SheetsSummarySender : MonoBehaviour
{
    private string webAppUrl = "https://script.google.com/macros/s/AKfycbybWZJTTkIEgTHcIChs8C1kWo_C9mgS5k6YwHD-GfJt2jJ399Ls-vpEyz9bmc8mhGwQtw/exec";

    public void SendGameSummary()
    {
        GameSummary summary = new GameSummary
        {
            playerName = GameManager.m_PlayerName,
            typeGame = GameManager.m_TypeGame? "Colors" : "Black",
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

[System.Serializable]
public class GameSummary
{
    public string playerName;
    public string typeGame;
    public float totalTime;
    public int totalClicks;
    public int totalRotations;
    public int levelsCompleted;
}

