using UnityEngine;

public class PartsAutoSetup : MonoBehaviour
{
    [Header("Setup Options")]
    [SerializeField] private bool addCollidersIfMissing = true;
    [SerializeField] private bool runOnStart = true;

    void Start()
    {
        if (runOnStart) SetupAllChildren();
    }

    [ContextMenu("Setup All Children")]
    public void SetupAllChildren()
    {
        int partsAdded = 0;
        int collidersAdded = 0;

        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>(true))
        {
            GameObject go = mr.gameObject;

            if (go.GetComponent<Part>() == null)
            {
                go.AddComponent<Part>();
                partsAdded++;
            }

            if (addCollidersIfMissing && go.GetComponent<Collider>() == null)
            {
                go.AddComponent<MeshCollider>();
                collidersAdded++;
            }
        }

        Debug.Log($"Added {partsAdded} Parts, {collidersAdded} MeshColliders");
    }
}