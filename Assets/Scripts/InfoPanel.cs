using UnityEngine;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    void Start()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void Show(Part part)
    {
        if (part == null || panelRoot == null) return;
        titleText.text = part.PartName;
        bodyText.text = part.Description;
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
