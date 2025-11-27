using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class MeshTangramCreator : MonoBehaviour
{
    public Transform m_FiguresParent;

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
    }
}
#endif
