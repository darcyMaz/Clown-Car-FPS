using UnityEngine;

public class PieSpawner : MonoBehaviour
{
    [SerializeField] private Transform Hand;
    [SerializeField] private Pie PieOriginal;

    private void Start()
    {
        EnemyMovement em;
        if (TryGetComponent(out em))
        {
            em.OnAttack += SpawnPie;
        }
    }

    // try get enemy component
    // connect to event
    // oh on awake get the hand transform or wrist transform or whatever
    // or make it serializedfield and that'll be in the prefab 
    // also on attack should pass the target transform X

    // LAST THING: tie it into event in animation

    private void SpawnPie(Vector3 target)
    {
        // instantiate pie at the wrist
        // look at the target
        // pass in the wrist and target

        Pie newPie = Instantiate<Pie>(PieOriginal, Hand.position, Quaternion.identity);
        newPie.SetHand(Hand);
        newPie.SetTarget(target);

    }
}
