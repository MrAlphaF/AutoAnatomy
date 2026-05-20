using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Part : MonoBehaviour
{
    [Header("Part Data")]
    [SerializeField] private string partName = "";
    [SerializeField, TextArea(3, 6)] private string description = "";

    [Header("Debug")]
    [SerializeField] private bool logSelection = true;

    private Renderer rend;
    private Material[] originalMaterials;
    private bool isHighlighted;

    public string PartName => string.IsNullOrEmpty(partName) ? gameObject.name : partName;
    public string Description => description;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.sharedMaterials;
    }

    public void ApplyHighlight(Material highlightMaterial)
    {
        if (isHighlighted || highlightMaterial == null) return;

        Material[] hl = new Material[originalMaterials.Length];
        for (int i = 0; i < hl.Length; i++) hl[i] = highlightMaterial;
        rend.materials = hl;
        isHighlighted = true;
    }

    public void RemoveHighlight()
    {
        if (!isHighlighted) return;
        rend.sharedMaterials = originalMaterials;
        isHighlighted = false;
    }

    public void OnSelected()
    {
        if (logSelection) Debug.Log($"Selected: {PartName}");
        // TODO: show info panel, start shoot-out animation
    }
}