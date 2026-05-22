using System.Collections;
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

    private Vector3 originalPosition;
    private Coroutine moveRoutine;

    public string PartName => string.IsNullOrEmpty(partName) ? gameObject.name : partName;
    public string Description => description;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.sharedMaterials;
        originalPosition = transform.position;
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
    }

    public void PullToward(Vector3 cameraPos, float distance, float duration)
    {
        Vector3 dir = (cameraPos - originalPosition).normalized;
        StartMove(originalPosition + dir * distance, duration);
    }

    public void ReturnToOrigin(float duration)
    {
        StartMove(originalPosition, duration);
    }

    private void StartMove(Vector3 target, float duration)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveTo(target, duration));
    }

    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 from = transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            transform.position = Vector3.Lerp(from, target, k);
            yield return null;
        }
        transform.position = target;
    }
}