using UnityEditor;
using UnityEngine;

public class FigureCreator : MonoBehaviour
{
    public void MakeSmallTriangleRectangle()
    {
        Mesh m = new Mesh();
        m.vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0)
        };

        m.triangles = new int[] { 0, 1, 2 };

        m.RecalculateNormals();
        m.RecalculateBounds();

        //string path = "Assets/Meshes/SmallTriangle.asset";
        //AssetDatabase.CreateAsset(m, path);
        //AssetDatabase.SaveAssets();
    }

    public void MakeParalelogram()
    {
        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),                  // A (origen)
            new Vector3(1, 0, 0),             // B (base)
            new Vector3(1, 1f, 0),       // C (desplazado por la hipotenusa)
            new Vector3(2f, 1f, 0)                 // D (lado superior)
        };

        m.triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2
        };

        m.RecalculateNormals();
        m.RecalculateBounds();

        //string path = "Assets/Meshes/Paralelogram.asset";
        //AssetDatabase.CreateAsset(m, path);
        //AssetDatabase.SaveAssets();
    }

    public void MakeMediumTriangleRectangle()
    {
        Mesh m = new Mesh();

        float cateto = Mathf.Sqrt(2f);

        m.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),          // A
            new Vector3(cateto, 0, 0),     // B
            new Vector3(0, cateto, 0)      // C
        };

        m.triangles = new int[] { 0, 1, 2 };

        m.RecalculateNormals();
        m.RecalculateBounds();

        //string path = "Assets/Meshes/MediumTriangle.asset";
        //AssetDatabase.CreateAsset(m, path);
        //AssetDatabase.SaveAssets();
    }

    public void MakeBigTriangleRectangle()
    {
        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),          // A
            new Vector3(2, 0, 0),     // B
            new Vector3(0, 2, 0)      // C
        };

        m.triangles = new int[] { 0, 1, 2 };

        m.RecalculateNormals();
        m.RecalculateBounds();

        //string path = "Assets/Meshes/BigTriangle.asset";
        //AssetDatabase.CreateAsset(m, path);
        //AssetDatabase.SaveAssets();
    }

    public void MakeSquare()
    {
        Mesh m = new Mesh();

        m.vertices = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0)
        };

        m.triangles = new int[] {
            0, 1, 2,
            0, 2, 3
        };

        m.RecalculateNormals();
        m.RecalculateBounds();

        //string path = "Assets/Meshes/Square.asset";
        //AssetDatabase.CreateAsset(m, path);
        //AssetDatabase.SaveAssets();
    }
}

// making a button in the inspector to call the MakeItTriangleRectangle function
#if UNITY_EDITOR
[CustomEditor(typeof(FigureCreator))]
public class TriangleRectangleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FigureCreator myScript = (FigureCreator)target;
        if (GUILayout.Button("Make Small Triangle"))
        {
            myScript.MakeSmallTriangleRectangle();
        }
        if (GUILayout.Button("Make Paralelogram "))
        {
            myScript.MakeParalelogram();
        }
        if (GUILayout.Button("Make Medium Triangle"))
        {
            myScript.MakeMediumTriangleRectangle();
        }
        if (GUILayout.Button("Make Big Triangle"))
        {
            myScript.MakeBigTriangleRectangle();
        }
        if (GUILayout.Button("Make Square"))
        {
            myScript.MakeSquare();
        }
    }
}
#endif