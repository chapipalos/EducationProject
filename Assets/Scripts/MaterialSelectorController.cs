using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelectorController : MonoBehaviour
{
    public GameObject m_FiguresParent;

    public MaterialSelector m_MaterialSelector;
    private void Awake()
    {
        if (m_FiguresParent == null)
        {
            Debug.LogWarning("m_FiguresParent no está asignado en MaterialSelectorController.");
            return;
        }
        if (m_MaterialSelector == null)
        {
            Debug.LogWarning("m_MaterialSelector no está asignado en MaterialSelectorController.");
            return;
        }

        List<Material> assignedMaterials = new List<Material>();
        assignedMaterials.AddRange(m_MaterialSelector.GetAllMaterials());

        foreach (Transform figureTransform in m_FiguresParent.transform)
        {
            Renderer renderer = figureTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                int randomIndex = Random.Range(0, assignedMaterials.Count);
                Material randomMat =  assignedMaterials[randomIndex];
                if (randomMat != null)
                {
                    renderer.material = randomMat;
                    assignedMaterials.Remove(randomMat);
                }
            }
        }
    }
}
