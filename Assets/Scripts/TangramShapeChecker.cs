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

    public Transform m_FiguresParent;
    private GameObject m_FigureToCheck;

    public GameObject m_CompletedText;

    public LevelsScriptableObject m_CurrentLevel;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(CompareMeshes);
        m_FigureToCheck = GameObject.FindGameObjectWithTag("Shape");
        matchThreshold = m_CurrentLevel.GetAllLevels()[GameManager.m_CurrentLevelIndex].m_MatchThreshold;
    }

    public void CompareMeshes()
    {
        if (m_FigureToCheck == null || m_FiguresParent == null) return;

        MeshFilter mfA = m_FigureToCheck.GetComponent<MeshFilter>();
        if (mfA == null || mfA.sharedMesh == null) return;

        Mesh targetMesh = mfA.sharedMesh;             // figura objetivo (shape blanca)
        Mesh placedMesh = GenerateTangramMesh();      // piezas colocadas combinadas

        var target = RasterizeMesh(targetMesh, m_FigureToCheck.transform.localToWorldMatrix);
        var placed = RasterizeMesh(placedMesh, Matrix4x4.identity); // ya está en world

        if (target.Count == 0 || placed.Count == 0) return;

        int intersection = 0;
        foreach (var cell in placed)
            if (target.Contains(cell))
                intersection++;

        float coverageTarget = (float)intersection / target.Count; // cuánto del objetivo está cubierto
        float coveragePlaced = (float)intersection / placed.Count; // cuánto de lo colocado cae dentro

        if (coverageTarget >= matchThreshold && coveragePlaced >= matchThreshold)
        {
            StartCoroutine(LoadNewLevel());
            Debug.Log("Level Completed!");
        }
        else
            Debug.Log("Keep Trying!");
    }

    HashSet<Vector2Int> RasterizeMesh(Mesh mesh, Matrix4x4 l2w)
    {
        float cell = Mathf.Max(0.001f, rasterCellSize);

        // Bounds en world (transformando sus 8 esquinas)
        Bounds b = mesh.bounds;

        Vector3[] corners =
        {
            new Vector3(b.min.x, b.min.y, b.min.z),
            new Vector3(b.min.x, b.min.y, b.max.z),
            new Vector3(b.min.x, b.max.y, b.min.z),
            new Vector3(b.min.x, b.max.y, b.max.z),
            new Vector3(b.max.x, b.min.y, b.min.z),
            new Vector3(b.max.x, b.min.y, b.max.z),
            new Vector3(b.max.x, b.max.y, b.min.z),
            new Vector3(b.max.x, b.max.y, b.max.z),
        };

        Vector3 wMin = l2w.MultiplyPoint3x4(corners[0]);
        Vector3 wMax = wMin;

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 w = l2w.MultiplyPoint3x4(corners[i]);
            wMin = Vector3.Min(wMin, w);
            wMax = Vector3.Max(wMax, w);
        }

        HashSet<Vector2Int> filled = new HashSet<Vector2Int>();

        // Muestreo en el bounding box
        for (float x = wMin.x; x <= wMax.x; x += cell)
        {
            for (float y = wMin.y; y <= wMax.y; y += cell)
            {
                Vector2 p = new Vector2(x, y);

                if (PointInsideMesh(p, mesh, l2w))
                {
                    int gx = Mathf.RoundToInt(x / cell);
                    int gy = Mathf.RoundToInt(y / cell);
                    filled.Add(new Vector2Int(gx, gy));
                }
            }
        }

        return filled;
    }

    bool PointInsideMesh(Vector2 worldPoint, Mesh mesh, Matrix4x4 l2w)
    {
        Vector3[] v = mesh.vertices;
        int[] t = mesh.triangles;

        for (int i = 0; i < t.Length; i += 3)
        {
            Vector2 a = (Vector2)l2w.MultiplyPoint3x4(v[t[i]]);
            Vector2 b = (Vector2)l2w.MultiplyPoint3x4(v[t[i + 1]]);
            Vector2 c = (Vector2)l2w.MultiplyPoint3x4(v[t[i + 2]]);

            if (PointInTriangle(worldPoint, a, b, c))
                return true;
        }
        return false;
    }

    bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // Barycentric
        Vector2 v0 = c - a;
        Vector2 v1 = b - a;
        Vector2 v2 = p - a;

        float dot00 = Vector2.Dot(v0, v0);
        float dot01 = Vector2.Dot(v0, v1);
        float dot02 = Vector2.Dot(v0, v2);
        float dot11 = Vector2.Dot(v1, v1);
        float dot12 = Vector2.Dot(v1, v2);

        float denom = dot00 * dot11 - dot01 * dot01;
        if (Mathf.Abs(denom) < 1e-8f) return false;

        float inv = 1f / denom;
        float u = (dot11 * dot02 - dot01 * dot12) * inv;
        float v = (dot00 * dot12 - dot01 * dot02) * inv;

        return (u >= 0f) && (v >= 0f) && (u + v <= 1f);
    }

    public Mesh GenerateTangramMesh()
    {
        MeshFilter[] filters = m_FiguresParent.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineList = new List<CombineInstance>();

        foreach (MeshFilter mf in filters)
        {
            if (mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix; // a world
            combineList.Add(ci);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combineList.ToArray(), true, true);
        finalMesh.RecalculateNormals();
        finalMesh.RecalculateBounds();

        return finalMesh;
    }

    public IEnumerator LoadNewLevel()
    {
        if (m_CompletedText != null)
            m_CompletedText.gameObject.SetActive(true);

        GameManager.m_LevelsCompleted++;
        GameManager.m_CurrentLevelIndex++;
        if (GameManager.m_LevelsCompleted >= m_CurrentLevel.GetAllLevels().Count)
        {
            GameManager.m_IsGameOver = true;
            yield return new WaitForSeconds(0f);
            SceneManager.LoadScene("Stats");
        }
        else
        {
            yield return new WaitForSeconds(3f);
            GameManager.m_TotalTime -= 3f;
            SceneManager.LoadScene("Game");
        }

    }
}
