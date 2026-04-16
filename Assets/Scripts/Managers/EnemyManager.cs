using System;
using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] private GameObject EnemyOriginal;

    private int CurrentWave = 0;

    public event Action <int> OnWaveDone;
    public event Action OnWaveStarted;

    private int EnemiesLeft = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        OnWaveDone += TempWaveDone;
    }
    private void OnDisable()
    {
        OnWaveDone -= TempWaveDone;
    }

    public void NextWave()
    {
        // I do not have the time to make this anything except an if else statement.
        CurrentWave++;
        if (CurrentWave == 1) Wave01();
        else if (CurrentWave == 2) Wave02();
        //else if (CurrentWave == 3) Wave03();
        //else if (CurrentWave == 4) Wave04();
        else
        {
            Debug.Log("No more waves to send.");
        }
    }

    private void Wave01()
    {
        // Spawn in three enemies on the dash

        List<EnemyHealth> eh_list = new List<EnemyHealth>();

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(0,75,-180), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(0, 75, -178), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(2, 75, -176), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(5, 75, -180), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(10, 75, -180), Quaternion.identity).GetComponent<EnemyHealth>());

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

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-20, 8, -50), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-15, 8, -50), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-10, 8, -70), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-12, 8, -60), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-15, 8, -65), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-18, 8, -48), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-8, 8, -55), Quaternion.identity).GetComponent<EnemyHealth>());
        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-25, 8, -60), Quaternion.identity).GetComponent<EnemyHealth>());



        // Every time the enemy dies, there is a check in this script to see if the wave is done.
        foreach (EnemyHealth e in eh_list)
        {
            e.OnEnemyDeath += EndFightCheck;
        }

        EnemiesLeft = eh_list.Count;
    }
    private void Wave03()
    {
        List<EnemyHealth> eh_list = new List<EnemyHealth>();

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(130, 40, -60), Quaternion.identity).GetComponent<EnemyHealth>());

        // Every time the enemy dies, there is a check in this script to see if the wave is done.
        foreach (EnemyHealth e in eh_list)
        {
            e.OnEnemyDeath += EndFightCheck;
        }

        EnemiesLeft = eh_list.Count;
    }
    private void Wave04()
    {
        List<EnemyHealth> eh_list = new List<EnemyHealth>();

        eh_list.Add(Instantiate(EnemyOriginal, new Vector3(-57, 12, 91), Quaternion.identity).GetComponent<EnemyHealth>());

        // Every time the enemy dies, there is a check in this script to see if the wave is done.
        foreach (EnemyHealth e in eh_list)
        {
            e.OnEnemyDeath += EndFightCheck;
        }

        EnemiesLeft = eh_list.Count;
    }

    private void EndFightCheck()
    {
        EnemiesLeft--;

        if (EnemiesLeft <= 0)
        {
            OnWaveDone?.Invoke(CurrentWave);
        }
    }

    private void TempWaveDone(int i)
    {
        NextWave();
    }
}
