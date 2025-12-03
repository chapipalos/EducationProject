using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorManager : MonoBehaviour
{
    public LevelsScriptableObject levelsSO;

    public Transform m_PositionsParent;
    private List<Transform> m_FiguresPositions = new List<Transform>();

    public Transform m_FiguresParent;

    public MaterialSelectorController m_MaterialController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < m_PositionsParent.childCount; i++)
        {
            m_FiguresPositions.Add(m_PositionsParent.GetChild(i));
        }

        LoadLevelFigures(GameManager.m_CurrentLevelIndex);

        m_MaterialController.SetColors();
    }

    private void LoadLevelFigures(int levelIndex)
    {
        LevelData currentLevel = levelsSO.GetAllLevels()[levelIndex];
        List<LevelsFigures> figures = currentLevel.m_Figures;
        int number = 0;
        foreach (LevelsFigures f in figures)
        {
            for(int i = 0; i < f.m_Amount; i++)
            {
                GameObject newFigure = Instantiate(f.m_Figure, m_FiguresPositions[number + i].position, Quaternion.identity, m_FiguresParent);
            }
            number += f.m_Amount;
        }

        GameObject figureTarget = Instantiate(currentLevel.m_LevelMesh, Vector3.zero, Quaternion.identity);
        figureTarget.name = currentLevel.m_LevelName;
        figureTarget.tag = "Shape";
    }
}
