using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Renderer))]
public class BulbEmissionSync : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color baseEmission = new Color(1f, 0.85f, 0.55f);
    [SerializeField] private float multiplier = 1.5f;

    private Light bulbLight;
    private Renderer rend;
    private Material matInstance;

    void Awake()
    {
        bulbLight = GetComponent<Light>();
        rend = GetComponent<Renderer>();
        matInstance = rend.material; // creates a unique copy so we don't edit the shared asset
    }

    void Update()
    {
        Color c = baseEmission * bulbLight.intensity * multiplier;
        matInstance.SetColor("_EmissionColor", c);
    }
}
