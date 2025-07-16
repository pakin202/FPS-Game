using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public enum Difficulty { Normal, Hard }

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Normal;

    [Header("Player Health")]
    public int normalPlayerHealth = 100;
    public int hardPlayerHealth = 120;

    [Header("Enemy Damage")]
    public int normalDamage = 10;
    public int hardDamage = 50;

    [Header("Enemy Attack Range")]
    public float normalAttackRange = 2f;
    public float hardAttackRange = 4f;

    public int GetEnemyDamage()
    {
        return difficulty == Difficulty.Normal ? normalDamage : hardDamage;
    }

    public int GetPlayerHealth()
    {
        return difficulty == Difficulty.Normal ? normalPlayerHealth : hardPlayerHealth;
    }

    public float GetEnemyAttackRange()
    {
        return difficulty == Difficulty.Normal ? normalAttackRange : hardAttackRange;
    }
}
