using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class MeshTangramCreator : MonoBehaviour
{
    public Transform m_FiguresParent;

    public Mesh m_TangramMesh;

    public MaterialSelector m_MaterialSelector;

    public string m_NameMesh;

    public void GenerateTangramMesh()
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

        string path = $"Assets/Meshes/Levels/{m_NameMesh}.asset";
        AssetDatabase.CreateAsset(finalMesh, path);
        AssetDatabase.SaveAssets();
    }

    public void PaintTriangles()
    {
        Mesh mesh = m_TangramMesh;
        var sourceVerts = mesh.vertices;
        var sourceTris = mesh.triangles;

        int triCount = sourceTris.Length / 3;

        // Evitar repetir colores
        List<Color> available = new List<Color>();
        foreach (var m in m_MaterialSelector.m_Materials)
            available.Add(m.color);

        // Si hay menos colores que triángulos, solo colorea los primeros
        int usableTriCount = Mathf.Min(triCount, available.Count);

        Vector3[] verts = new Vector3[sourceTris.Length];
        int[] tris = new int[sourceTris.Length];
        Color[] colors = new Color[sourceTris.Length];

        // Barajar colores para asignarlos sin repetición
        Shuffle(available);

        for (int t = 0; t < triCount; t++)
        {
            Color triColor =
                t < usableTriCount ? available[t] : Color.white; // fallback si faltan colores

            int baseIndex = t * 3;

            for (int k = 0; k < 3; k++)
            {
                int idx = baseIndex + k;

                verts[idx] = sourceVerts[sourceTris[idx]];
                tris[idx] = idx;
                colors[idx] = triColor;    // mismo color para los 3 vértices
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int r = Random.Range(0, i + 1);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public void ResetColors()
    {
        Mesh mesh = m_TangramMesh;
        mesh.colors = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshTangramCreator))]
public class MeshTangramCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshTangramCreator myScript = (MeshTangramCreator)target;
        if (GUILayout.Button("Generate Tangram Mesh"))
        {
            myScript.GenerateTangramMesh();
        }

        if (GUILayout.Button("Paint Triangles"))
        {
            myScript.PaintTriangles();
        }

        if (GUILayout.Button("Reset Colors"))
        {
            myScript.ResetColors();
        }
    }
}
#endif
