using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Game Settings")]
    public GameSettings gameSettings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetNormalDifficulty() 
    {
        gameSettings.difficulty = GameSettings.Difficulty.Normal;
    }

    public void SetHardDifficulty() 
    {
        gameSettings.difficulty = GameSettings.Difficulty.Hard;
    }
}
