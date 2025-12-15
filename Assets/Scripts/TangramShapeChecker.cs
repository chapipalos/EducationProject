using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TangramShapeChecker : MonoBehaviour
{
    [Header("Rasterizado")]
    [Tooltip("Más pequeño = más preciso, más lento")]
    public float rasterCellSize = 0.05f;

    [Header("Umbral de coincidencia")]
    [Range(0.8f, 1f)]
    [Tooltip("0.97 suele ir bien para bordes compartidos")]
    public float matchThreshold = 0.97f;

    [Header("Referencias")]
    public Transform m_FiguresParent;
    public GameObject m_CompletedText;
    public LevelsScriptableObject m_CurrentLevel;

    [Header("Botones")]
    public Button checkButton;
    public Button skipButton;

    private GameObject m_FigureToCheck;

    private void Start()
    {
        m_FigureToCheck = GameObject.FindGameObjectWithTag("Shape");

        if (m_CurrentLevel != null && GameManager.m_CurrentLevelIndex < m_CurrentLevel.GetAllLevels().Count)
        {
            matchThreshold = m_CurrentLevel.GetAllLevels()[GameManager.m_CurrentLevelIndex].m_MatchThreshold;
        }

        // Configurar botones
        if (checkButton != null)
            checkButton.onClick.AddListener(CheckShape);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipLevel);
    }

    // Verifica si las piezas del tangram coinciden con la figura objetivo
    public void CheckShape()
    {
        if (!ValidateReferences()) return;

        MeshFilter mfTarget = m_FigureToCheck.GetComponent<MeshFilter>();
        if (mfTarget == null || mfTarget.sharedMesh == null)
        {
            Debug.LogWarning("No se pudo obtener el mesh de la figura objetivo");
            return;
        }

        Mesh targetMesh = mfTarget.sharedMesh;
        Mesh placedMesh = GenerateTangramMesh();

        var targetCells = RasterizeMesh(targetMesh, m_FigureToCheck.transform.localToWorldMatrix);
        var placedCells = RasterizeMesh(placedMesh, Matrix4x4.identity);

        if (targetCells.Count == 0 || placedCells.Count == 0)
        {
            Debug.LogWarning("No se pudieron rasterizar las mallas correctamente");
            return;
        }

        // Calcular intersección
        int intersection = 0;
        foreach (var cell in placedCells)
        {
            if (targetCells.Contains(cell))
                intersection++;
        }

        float coverageTarget = (float)intersection / targetCells.Count;
        float coveragePlaced = (float)intersection / placedCells.Count;

        Debug.Log($"Cobertura objetivo: {coverageTarget:P2} | Cobertura colocada: {coveragePlaced:P2}");

        if (coverageTarget >= matchThreshold && coveragePlaced >= matchThreshold)
        {
            OnLevelCompleted();
        }
        else
        {
            OnLevelFailed();
        }
    }

    // Salta el nivel actual sin completarlo
    public void SkipLevel()
    {
        StartCoroutine(LoadNextLevel(false));
        Debug.Log("Level Skipped!");
    }

    private void OnLevelCompleted()
    {
        Debug.Log("¡Nivel Completado!");
        StartCoroutine(LoadNextLevel(true));
    }

    private void OnLevelFailed()
    {
        Debug.Log("¡Sigue intentándolo!");
    }

    private bool ValidateReferences()
    {
        if (m_FigureToCheck == null)
        {
            Debug.LogError("No se encontró la figura objetivo con tag 'Shape'");
            return false;
        }

        if (m_FiguresParent == null)
        {
            Debug.LogError("FiguresParent no está asignado");
            return false;
        }

        return true;
    }

    private HashSet<Vector2Int> RasterizeMesh(Mesh mesh, Matrix4x4 localToWorld)
    {
        float cell = Mathf.Max(0.001f, rasterCellSize);
        HashSet<Vector2Int> filled = new HashSet<Vector2Int>();

        // Calcular bounds en espacio mundo
        Bounds bounds = mesh.bounds;
        Vector3[] corners = GetBoundsCorners(bounds);

        Vector3 worldMin = localToWorld.MultiplyPoint3x4(corners[0]);
        Vector3 worldMax = worldMin;

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 worldPoint = localToWorld.MultiplyPoint3x4(corners[i]);
            worldMin = Vector3.Min(worldMin, worldPoint);
            worldMax = Vector3.Max(worldMax, worldPoint);
        }

        // Muestreo del área
        for (float x = worldMin.x; x <= worldMax.x; x += cell)
        {
            for (float y = worldMin.y; y <= worldMax.y; y += cell)
            {
                Vector2 point = new Vector2(x, y);

                if (PointInsideMesh(point, mesh, localToWorld))
                {
                    int gridX = Mathf.RoundToInt(x / cell);
                    int gridY = Mathf.RoundToInt(y / cell);
                    filled.Add(new Vector2Int(gridX, gridY));
                }
            }
        }

        return filled;
    }

    private Vector3[] GetBoundsCorners(Bounds bounds)
    {
        return new Vector3[]
        {
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z),
        };
    }

    private bool PointInsideMesh(Vector2 worldPoint, Mesh mesh, Matrix4x4 localToWorld)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector2 a = localToWorld.MultiplyPoint3x4(vertices[triangles[i]]);
            Vector2 b = localToWorld.MultiplyPoint3x4(vertices[triangles[i + 1]]);
            Vector2 c = localToWorld.MultiplyPoint3x4(vertices[triangles[i + 2]]);

            if (PointInTriangle(worldPoint, a, b, c))
                return true;
        }

        return false;
    }

    private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // Coordenadas baricéntricas
        Vector2 v0 = c - a;
        Vector2 v1 = b - a;
        Vector2 v2 = p - a;

        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        float denominator = dot00 * dot11 - dot01 * dot01;
        if (Mathf.Abs(denominator) < 1e-8f) return false;

        float invDenom = 1f / denominator;
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0f) && (v >= 0f) && (u + v <= 1f);
    }

    private Mesh GenerateTangramMesh()
    {
        MeshFilter[] filters = m_FiguresParent.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineList = new List<CombineInstance>();

        foreach (MeshFilter mf in filters)
        {
            if (mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            combineList.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineList.ToArray(), true, true);
        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();

        return combinedMesh;
    }

    private IEnumerator LoadNextLevel(bool completed)
    {
        // Mostrar texto de completado si corresponde
        if (completed && m_CompletedText != null)
        {
            m_CompletedText.SetActive(true);
        }

        // Actualizar contadores
        if (completed)
            GameManager.m_LevelsCompleted++;
        else
            GameManager.m_LevelsSkipped++;

        // Avanzar al siguiente nivel
        GameManager.m_CurrentLevelIndex++;

        // Wrap around si llegamos al final
        if (GameManager.m_CurrentLevelIndex >= m_CurrentLevel.GetAllLevels().Count)
        {
            GameManager.m_CurrentLevelIndex = 0;
        }

        // Verificar si terminamos todos los niveles
        int totalProcessed = GameManager.m_LevelsCompleted + GameManager.m_LevelsSkipped;
        if (totalProcessed >= m_CurrentLevel.GetAllLevels().Count)
        {
            GameManager.m_IsGameOver = true;
            yield return new WaitForSeconds(0f);
            SceneManager.LoadScene("Stats");
        }
        else
        {
            float waitTime = completed ? 3f : 0f;
            yield return new WaitForSeconds(waitTime);

            if (completed)
                GameManager.m_TotalTime -= 3f;

            SceneManager.LoadScene("Game");
        }
    }
}