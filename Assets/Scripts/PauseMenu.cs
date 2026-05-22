using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PartInteractor partInteractor;

    private bool isPaused;

    void Start()
    {
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        LockCursor(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // don't pause while inspecting a part (right-click exits inspection first)
            if (partInteractor != null && partInteractor.IsInspecting) return;
            if (isPaused) Resume(); else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
        if (playerController != null) playerController.enabled = false;
        Time.timeScale = 0f;
        LockCursor(false);
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        Time.timeScale = 1f;
        LockCursor(true);
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
