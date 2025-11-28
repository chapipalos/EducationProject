using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelsSO", menuName = "Scriptable Objects/LevelsSO")]
public class LevelsScriptableObject : ScriptableObject
{
    public List<LevelData> m_Levels = new List<LevelData>();

    public List<LevelData> GetAllLevels()
    {
        return m_Levels;
    }

    public LevelData GetLevelByName(string levelName)
    {
        return m_Levels.Find(level => level.m_LevelName == levelName);
    }
}

[System.Serializable]
public class LevelData
{
    public string m_LevelName;
    public GameObject m_LevelMesh;
    public List<LevelsFigures> m_Figures = new List<LevelsFigures>();
}

[System.Serializable]
public class LevelsFigures
{
    public GameObject m_Figure;
    public int m_Amount;
}
