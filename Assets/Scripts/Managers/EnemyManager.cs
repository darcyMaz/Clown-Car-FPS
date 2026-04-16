using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject EnemyOriginal;

    private int CurrentWave = 0;

    public event Action <int> OnWaveDone;
    public event Action OnWaveStarted;

    private int EnemiesLeft = 0;

    public void NextWave()
    {
        // I do not have the time to make this anything except an if else statement.
        CurrentWave++;
        if (CurrentWave == 1) Wave01();
        else if (CurrentWave == 2) Wave02();
        else if (CurrentWave == 3) Wave03();
        else if (CurrentWave == 4) Wave04();
    }

    private void Wave01()
    {
        // Spawn in three enemies on the dash

        List<EnemyHealth> eh_list = new List<EnemyHealth>();

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(0,75,-180), Quaternion.identity).GetComponent<EnemyHealth>());

        // Every time the enemy dies, there is a check in this script to see if the wave is done.
        foreach (EnemyHealth e in eh_list)
        {
            e.OnEnemyDeath += EndFightCheck;
        }

        EnemiesLeft = eh_list.Count;
    }
    private void Wave02()
    {
        // Spawn in 6 enemies on the ground

        List<EnemyHealth> eh_list = new List<EnemyHealth>();

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-15, 8, -50), Quaternion.identity).GetComponent<EnemyHealth>());

        // Every time the enemy dies, there is a check in this script to see if the wave is done.
        foreach (EnemyHealth e in eh_list)
        {
            e.OnEnemyDeath += EndFightCheck;
        }

        EnemiesLeft = eh_list.Count;
    }
    private void Wave03()
    {

    }
    private void Wave04()
    {

    }

    private void EndFightCheck()
    {
        EnemiesLeft--;

        if (EnemiesLeft <= 0)
        {
            OnWaveDone?.Invoke(CurrentWave);
        }
    }
}
