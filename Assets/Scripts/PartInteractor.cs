using UnityEngine;

public class PartInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("Highlight")]
    [SerializeField] private Material highlightMaterial;

    [Header("Animation")]
    [SerializeField] private float pullDistance = 0.4f;
    [SerializeField] private float animationDuration = 0.5f;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InfoPanel infoPanel;

    private Part hoveredPart;
    private Part selectedPart;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        UpdateHover();
        if (Input.GetMouseButtonDown(0)) HandleClick();
        if (Input.GetMouseButtonDown(1)) Deselect();
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
        // Leave the selected part's highlight alone
        if (hoveredPart != null && hoveredPart != selectedPart) hoveredPart.RemoveHighlight();
        hoveredPart = newPart;
        if (hoveredPart != null && hoveredPart != selectedPart) hoveredPart.ApplyHighlight(highlightMaterial);
    }

    private void HandleClick()
    {
        if (hoveredPart == null) return;

        // Send the previous selection back first
        if (selectedPart != null && selectedPart != hoveredPart)
        {
            selectedPart.ReturnToOrigin(animationDuration);
            selectedPart.RemoveHighlight();
        }

        selectedPart = hoveredPart;
        selectedPart.OnSelected();
        selectedPart.ApplyHighlight(highlightMaterial);
        selectedPart.PullToward(playerCamera.transform.position, pullDistance, animationDuration);

        if (infoPanel != null) infoPanel.Show(selectedPart);
    }

    private void Deselect()
    {
        if (selectedPart != null)
        {
            selectedPart.ReturnToOrigin(animationDuration);
            selectedPart.RemoveHighlight();
            selectedPart = null;
        }
        if (infoPanel != null) infoPanel.Hide();
    }
}