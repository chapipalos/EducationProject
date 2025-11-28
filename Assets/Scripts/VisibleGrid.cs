using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteAlways]
public class VisibleGrid : MonoBehaviour
{
    public float cellSize = 1f;
    public int width = 20;
    public int height = 20;
    public Color gridColor = Color.gray;
    public float lineWidth = 0.02f;

    private void OnEnable()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Limpia líneas antiguas
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        float halfW = (width * cellSize) * 0.5f;
        float halfH = (height * cellSize) * 0.5f;

        // Líneas verticales (centradas)
        for (int x = 0; x <= width; x++)
        {
            float xPos = -halfW + x * cellSize;
            CreateLine(
                new Vector3(xPos, -halfH, 0),
                new Vector3(xPos, halfH, 0)
            );
        }

        // Líneas horizontales (centradas)
        for (int y = 0; y <= height; y++)
        {
            float yPos = -halfH + y * cellSize;
            CreateLine(
                new Vector3(-halfW, yPos, 0),
                new Vector3(halfW, yPos, 0)
            );
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(transform, false);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    public void destroyChildren()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
            i--;
        }
    }

#if UNITY_EDITOR
[CustomEditor(typeof(VisibleGrid))]
    public class TriangleRectangleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            VisibleGrid myScript = (VisibleGrid)target;
            if (GUILayout.Button("Eliminate Children"))
            {
                myScript.destroyChildren();
            }
        }
    }
#endif
}
