using UnityEngine;

public class PartInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("Highlight")]
    [SerializeField] private Material highlightMaterial;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InfoPanel infoPanel;

    private Part hoveredPart;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        UpdateHover();
        if (Input.GetMouseButtonDown(0)) HandleClick();
        if (Input.GetMouseButtonDown(1) && infoPanel != null) infoPanel.Hide();
    }

    private void UpdateHover()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayers))
        {
            Part part = hit.collider.GetComponentInParent<Part>();
            if (part != hoveredPart) SetHovered(part);
        }
        else if (hoveredPart != null)
        {
            SetHovered(null);
        }
    }

    private void SetHovered(Part newPart)
    {
        if (hoveredPart != null) hoveredPart.RemoveHighlight();
        hoveredPart = newPart;
        if (hoveredPart != null) hoveredPart.ApplyHighlight(highlightMaterial);
    }

    private void HandleClick()
    {
        if (hoveredPart == null) return;
        hoveredPart.OnSelected();
        if (infoPanel != null) infoPanel.Show(hoveredPart);
    }
}