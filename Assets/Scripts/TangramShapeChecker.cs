using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TangramShapeChecker : MonoBehaviour
{
    [Header("Tolerancias")]
    public float areaTolerance = 0.01f;
    public float hullTolerance = 0.01f;

    public Transform m_FiguresParent;
    private GameObject m_FigureToCheck;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(CompareMeshes);
        m_FigureToCheck = GameObject.FindGameObjectWithTag("Shape");
    }

    public void CompareMeshes()
    {
        Mesh meshA = m_FigureToCheck.GetComponent<MeshFilter>().mesh;
        Mesh meshB = GenerateTangramMesh();

        // --- Proyección a 2D ---
        Vector2[] vertsA = ProjectTo2D(meshA);
        Vector2[] vertsB = ProjectTo2D(meshB);

        // --- Área ---
        float areaA = PolygonArea(vertsA, meshA.triangles);
        float areaB = PolygonArea(vertsB, meshB.triangles);

        if (Mathf.Abs(areaA - areaB) > areaTolerance)
        {
            Debug.Log(false);
            return;
        }

        // --- Convex Hull ---
        List<Vector2> hullA = ConvexHull(new List<Vector2>(vertsA));
        List<Vector2> hullB = ConvexHull(new List<Vector2>(vertsB));

        if (!CompareHulls(hullA, hullB, hullTolerance))
        {
            Debug.Log(false);
            return;
        }

        Debug.Log(true);
    }

    Vector2[] ProjectTo2D(Mesh mesh)
    {
        Vector3[] verts = mesh.vertices;
        Vector2[] verts2D = new Vector2[verts.Length];

        for (int i = 0; i < verts.Length; i++)
            verts2D[i] = new Vector2(verts[i].x, verts[i].y);

        return verts2D;
    }

    float PolygonArea(Vector2[] verts, int[] tris)
    {
        float area = 0f;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector2 a = verts[tris[i]];
            Vector2 b = verts[tris[i + 1]];
            Vector2 c = verts[tris[i + 2]];

            area += Mathf.Abs(
                (a.x * (b.y - c.y) +
                 b.x * (c.y - a.y) +
                 c.x * (a.y - b.y)) * 0.5f
            );
        }

        return area;
    }

    List<Vector2> ConvexHull(List<Vector2> points)
    {
        if (points.Count <= 1) return points;

        points.Sort((a, b) =>
            a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x)
        );

        List<Vector2> hull = new List<Vector2>();

        // Lower hull
        foreach (var p in points)
        {
            while (hull.Count >= 2 &&
                   Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);

            hull.Add(p);
        }

        // Upper hull
        int t = hull.Count + 1;
        for (int i = points.Count - 1; i >= 0; i--)
        {
            var p = points[i];
            while (hull.Count >= t &&
                   Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);

            hull.Add(p);
        }

        hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) -
               (a.y - o.y) * (b.x - o.x);
    }

    bool CompareHulls(List<Vector2> a, List<Vector2> b, float tolerance)
    {
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (Vector2.Distance(a[i], b[i]) > tolerance)
                return false;
        }

        return true;
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
            ci.transform = mf.transform.localToWorldMatrix;
            combineList.Add(ci);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combineList.ToArray(), true, true);
        finalMesh.RecalculateNormals();
        finalMesh.RecalculateBounds();

        return finalMesh;
    }
}
