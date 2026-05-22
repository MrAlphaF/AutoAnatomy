using UnityEngine;

public class PartInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("Highlight")]
    [SerializeField] private Material highlightMaterial;

    [Header("Inspection")]
    [SerializeField] private float fillFactor = 0.8f;        // lower = part fills more of the screen
    [SerializeField] private float animationDuration = 0.4f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float clearance = 0.05f;        // extra gap kept in front of the near clip plane

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InfoPanel infoPanel;
    [SerializeField] private PlayerController playerController;

    private Part hoveredPart;
    private Part selectedPart;
    private bool isInspecting;
    private Vector3 inspectCenter;
    private float rotateReadyTime;

    public bool IsInspecting => isInspecting;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerController == null) playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isInspecting)
        {
            HandleRotation();
            if (Input.GetMouseButtonDown(1)) ExitInspection();
        }
        else
        {
            UpdateHover();
            if (Input.GetMouseButtonDown(0)) EnterInspection();
        }
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

    private void EnterInspection()
    {
        if (hoveredPart == null) return;

        selectedPart = hoveredPart;
        selectedPart.OnSelected();
        selectedPart.RemoveHighlight();

        Bounds b = selectedPart.GetWorldBounds();
        float radius = b.extents.magnitude;
        float fov = playerCamera.fieldOfView * Mathf.Deg2Rad;
        float distance = (radius / Mathf.Sin(fov * 0.5f)) * fillFactor;

        float minDistance = playerCamera.nearClipPlane + radius + clearance;
        distance = Mathf.Max(distance, minDistance);

        inspectCenter = playerCamera.transform.position + playerCamera.transform.forward * distance;

        Vector3 offset = selectedPart.transform.position - b.center;
        selectedPart.MoveTo(inspectCenter + offset, animationDuration);

        if (playerController != null) playerController.enabled = false;
        if (infoPanel != null) infoPanel.Show(selectedPart);

        isInspecting = true;
        hoveredPart = null;
        rotateReadyTime = Time.time + animationDuration;
    }

    private void HandleRotation()
    {
        if (selectedPart == null || Time.time < rotateReadyTime) return;

        float mx = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        Transform t = selectedPart.transform;
        t.RotateAround(inspectCenter, Vector3.up, -mx);
        t.RotateAround(inspectCenter, playerCamera.transform.right, my);
    }

    private void ExitInspection()
    {
        if (selectedPart != null)
        {
            selectedPart.ReturnToOrigin(animationDuration);
            selectedPart = null;
        }
        if (playerController != null) playerController.enabled = true;
        if (infoPanel != null) infoPanel.Hide();
        isInspecting = false;
    }
}