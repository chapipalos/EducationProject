using System.Collections.Generic;
using UnityEngine;

public class LevelSelectorManager : MonoBehaviour
{
    public LevelsScriptableObject levelsSO;

    public Transform m_PositionsParent;
    private List<Transform> m_FiguresPositions = new List<Transform>();

    public Transform m_FiguresParent;

    public MaterialSelectorController m_MaterialController;

    private VisibleGrid grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        grid = FindFirstObjectByType<VisibleGrid>();

        for (int i = 0; i < m_PositionsParent.childCount; i++)
        {
            m_FiguresPositions.Add(m_PositionsParent.GetChild(i));
        }

        LoadLevelFigures(GameManager.m_CurrentLevelIndex);
    }

    public void LoadLevelFigures(int levelIndex)
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
        figureTarget.transform.position = SnapToGrid(figureTarget.transform.position);

        m_MaterialController.SetColors();
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        float size = grid.cellSize;

        float x = Mathf.Round((pos.x - grid.transform.position.x) / size) * size + grid.transform.position.x;
        float y = Mathf.Round((pos.y - grid.transform.position.y) / size) * size + grid.transform.position.y;

        return new Vector3(x, y, pos.z);
    }
}
