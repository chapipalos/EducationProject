using System.Collections.Generic;
using UnityEngine;

public class TangramShapeChecker : MonoBehaviour
{
    [Header("Solución (Valores esperados)")]
    public float targetArea = 1f;
    public float areaTolerance = 0.01f;

    public List<Vector2> targetHull = new List<Vector2>();
    public float hullTolerance = 0.01f;

    public bool CheckTangramSolution(Mesh tangramMesh)
    {
        // 1. Bounding
        Bounds b = tangramMesh.bounds;

        // 2. Proyección a 2D
        Vector2[] verts2D = ProjectTo2D(tangramMesh);

        // 3. Área
        float area = PolygonArea(verts2D, tangramMesh.triangles);

        if (Mathf.Abs(area - targetArea) > areaTolerance)
            return false;

        // 4. Convex hull
        List<Vector2> hull = ConvexHull(new List<Vector2>(verts2D));

        if (!CompareHulls(hull, targetHull, hullTolerance))
            return false;

        return true;
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

        foreach (var p in points)
        {
            while (hull.Count >= 2 &&
                   Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);

            hull.Add(p);
        }

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
}
