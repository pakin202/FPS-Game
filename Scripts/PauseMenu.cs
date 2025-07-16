using UnityEngine;
using StarterAssets; // ✅ ใช้ StarterAssetsInputs

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;
    private StarterAssetsInputs starterAssetsInputs;
    private FirstPersonController playerController; // ✅ เพิ่มตัวแปรควบคุมผู้เล่น

    void Start()
    {
        starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();
        playerController = FindObjectOfType<FirstPersonController>(); // ✅ หา Controller ของผู้เล่น
    }

    void Update()
    {
        if (starterAssetsInputs != null && starterAssetsInputs.pause)
        {
            TogglePause();
            starterAssetsInputs.PauseInput(false); // ✅ รีเซ็ตค่า input
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f; // ✅ หยุด/เล่นเกม

        if (playerController != null)
        {
            playerController.enabled = !isPaused; // ✅ ปิด/เปิดการควบคุมกล้อง
        }

        Cursor.visible = isPaused; // ✅ แสดงเคอร์เซอร์เมื่อ Pause
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked; // ✅ ล็อก/ปลดล็อกเคอร์เซอร์
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
