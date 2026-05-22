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
    private Quaternion originalRotation;
    private Coroutine moveRoutine;

    public string PartName => string.IsNullOrEmpty(partName) ? gameObject.name : partName;
    public string Description => description;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterials = rend.sharedMaterials;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public Bounds GetWorldBounds() => rend.bounds;

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

    public void MoveTo(Vector3 target, float duration)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine(target, transform.rotation, duration));
    }

    public void ReturnToOrigin(float duration)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine(originalPosition, originalRotation, duration));
    }

    private IEnumerator MoveRoutine(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 fromPos = transform.position;
        Quaternion fromRot = transform.rotation;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            transform.position = Vector3.Lerp(fromPos, targetPos, k);
            transform.rotation = Quaternion.Slerp(fromRot, targetRot, k);
            yield return null;
        }
        transform.position = targetPos;
        transform.rotation = targetRot;
    }
}