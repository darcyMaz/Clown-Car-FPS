using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Health Data
    [SerializeField] private int MaxHealth = 10;
    private int health;

    // C# Events
    public event Action <int> OnHealthChanged;
    public event Action OnEnemyDeath;

    private void OnEnable()
    {
        OnEnemyDeath += EnemyDeath;
    }
    private void OnDisable()
    {
        OnEnemyDeath -= EnemyDeath;
    }

    private void Start()
    {
        health = MaxHealth;
    }

    public void ChangeHealth(int diff)
    {
        int prevHealth = health;

        health = (health + diff < 0) ? 0: (health + diff > MaxHealth) ? MaxHealth: health + diff;

        if (prevHealth != health)
        {
            // Invoke Change Health
            // Send the difference between the new and previous health. Do not send the diff variable because it is not clamped to 0 or MaxHealth
            OnHealthChanged?.Invoke(health - prevHealth);
        }

        if (health <= 0)
        {
            // Invoke Death
            OnEnemyDeath?.Invoke();
        }
    }

    private void EnemyDeath()
    {
        Destroy(gameObject);
    }
}
