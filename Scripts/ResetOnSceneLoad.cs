using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetOnSceneLoad : MonoBehaviour
{
    // สมมติว่ามีสคริปต์ควบคุมการเคลื่อนที่ที่ต้อง re-enable
    [SerializeField] private MonoBehaviour movementScript;
    [SerializeField] private MonoBehaviour aiScript; // สำหรับ enemy หรือ NPC

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (movementScript != null)
            movementScript.enabled = true;
        if (aiScript != null)
            aiScript.enabled = true;
    }
}
