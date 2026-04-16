using System;
using UnityEngine;

public class PieSpawner : MonoBehaviour
{
    [SerializeField] private Transform Hand;
    [SerializeField] private Pie PieOriginal;

    private EnemyMovement em;
    private bool HasEM = true;


    private void Start()
    {
        if (TryGetComponent(out em))
        {
            em.OnAttack += SpawnPie;
        }
        else HasEM = false;
    }

    private void OnDisable()
    {
        if (HasEM)
        {
            em.OnAttack -= SpawnPie;
        }
    }

    // try get enemy component X
    // connect to event X
    // or make it serializedfield and that'll be in the prefab X
    // also on attack should pass the target transform X

    // LAST THING: tie it into event in animation

    private void SpawnPie(Vector3 target)
    {
        // instantiate pie at the hand
        // look at the target
        // pass in the wrist and target

        Pie newPie = Instantiate<Pie>(PieOriginal, Hand.position, Quaternion.identity);
        newPie.SetHand(Hand);
        newPie.SetTarget(target);
        if (HasEM) newPie.SetEnemyMovement(em);
    }
}
