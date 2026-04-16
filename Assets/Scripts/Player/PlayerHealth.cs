using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int MaxHealth;

    public event Action <int> OnHealthChange;
    public event Action OnDeath;

    private void OnEnable()
    {
        OnDeath += Death;
    }
    private void OnDisable()
    {
        OnDeath -= Death;
    }

    public void ChangeHealth(int diff)
    {
        //int prevHealth = health;
        health = (health + diff < 0) ? 0 : (health + diff > MaxHealth) ? MaxHealth : health + diff;

        OnHealthChange?.Invoke(health);
        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    private void Death()
    {
        SceneManager.Instance.BufferSceneChange("Start Menu");
    }
}
