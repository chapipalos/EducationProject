using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialSelector", menuName = "Scriptable Objects/MaterialsPool")]
public class MaterialSelector : ScriptableObject
{
    public List<Material> m_Materials = new List<Material>();

    public List<Material> GetAllMaterials()
    {
        return m_Materials;
    }
}
