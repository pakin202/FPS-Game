using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonEvent : MonoBehaviour
{
    [Header("Scene Settings")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAssetTransition;
    [SerializeField] private SceneAsset sceneAssetRestart;
#else
    [SerializeField] private string sceneAssetTransition;
    [SerializeField] private string sceneAssetRestart;
#endif

    private string sceneToLoad;
    private string sceneToRestart;

    [Header("UI Settings")]
    [SerializeField] private GameObject settingPanel; // ตัวแปรเก็บ Panel

     [Header("Difficulty Settings")]
    public DifficultyManager difficultyManager;

    public void StartGameWithNormalDifficulty()
    {
        difficultyManager.SetNormalDifficulty();
        SceneTransition();
    }

    public void StartGameWithHardDifficulty()
    {
        difficultyManager.SetHardDifficulty();
        SceneTransition();
    }

    private void Awake()
    {
#if UNITY_EDITOR
        if (sceneAssetTransition != null)
        {
            sceneToLoad = sceneAssetTransition.name;
        }
        if (sceneAssetRestart != null)
        {
            sceneToRestart = sceneAssetRestart.name; 
        }
#else
        sceneToLoad = sceneAssetTransition;
        sceneToRestart = sceneAssetRestart;
#endif
    }

    // รีสตาร์ทเกม
    public void RestartGame()
    {
        Time.timeScale = 1; 
        if (!string.IsNullOrEmpty(sceneToRestart))
        {
            SceneManager.LoadScene(sceneToRestart);
        }
        else
        {
            Debug.LogError("Scene to restart is not set!");
        }
        Debug.Log("Press RestartGame");
    }

    // เปลี่ยน Scene
    public void SceneTransition()
{
    Time.timeScale = 1; 
    // ซ่อนเคอร์เซอร์ก่อนเปลี่ยน Scene
    Cursor.visible = false; // ซ่อนเคอร์เซอร์
    Cursor.lockState = CursorLockMode.Locked; // ปิดการเคลื่อนที่ของเคอร์เซอร์

    if (!string.IsNullOrEmpty(sceneToLoad))
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    else
    {
        Debug.LogError("Scene to transition is not set!");
    }
}


    // เปิดหน้า Setting
    public void OpenSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            Time.timeScale = 0; // หยุดเกมขณะเปิด Setting
        }
    }

    // ปิดหน้า Setting
    public void CloseSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
            Time.timeScale = 1; // กลับมาเล่นเกมปกติ
        }
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
        // ถ้าใน Unity Editor ให้หยุดการเล่น
        EditorApplication.isPlaying = false;
        #else
        // ถ้าในโหมดสร้างเกมจริง ให้ใช้คำสั่งนี้เพื่อออกจากเกม
        Application.Quit();
        #endif
    }
}
