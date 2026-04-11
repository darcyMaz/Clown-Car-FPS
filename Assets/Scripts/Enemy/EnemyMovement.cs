using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // make this a navmeshagent
    // make some states
    // do a sphere check for the player
    // if we are with run range, run to them.
    // if we are within throwing range, throw pie at them
    // simple as

    // Navmesh Agent
    private NavMeshAgent agent;
    private bool HasAgent = false;

    // Search vars
    [SerializeField] private float CircleSearch = 50f;
    private float ThrowDistance = 0f;
    [SerializeField] private LayerMask PlayerLayer;
    private Collider target;

    // Misc vars
    [SerializeField] private int MaxPlayersInGame = 12;

    // I want two distances
    // 1) Circle search distance
    // 2) Path distance
    // Search for players in the circle distance
    // If we find some, choose the closest one, and then see if it within the path distance.
    // If it is, go towards it!

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (!TryGetComponent(out agent)) Debug.Log("An EnemyMovement Script could not find its corresponding NavMeshAgent component.");
        else HasAgent = true;
    }

    private void Start()
    {
        if (HasAgent)
        {
            // The Enemy will attack once it reaches the stop distance.
            // It's possible that there needs to be a bit of tolerance for this to work.
            ThrowDistance = agent.stoppingDistance;

            FindTarget();
        }
    }

    private void FixedUpdate()
    {
        if (HasAgent)
        {
            if (FindTarget())
            {
                // Go towards target.
                // Ok so do i want to restart search every frame??? Uuuh yes?
            }
        }
    }

    private bool FindTarget()
    {
        //Collider[] targetsHit = Physics.OverlapSphereNonAlloc(transform.position, CircleSearch, PlayerLayer);
        Collider[] targetsHit = new Collider[MaxPlayersInGame];
        int hitTotal = Physics.OverlapSphereNonAlloc(transform.position, CircleSearch, targetsHit);

        // Go through each target hit and find the closest one.
        // This assumes that the targetsHit array at 0 is not always the closest one.
        // I will assume it is for brevity.

        if (hitTotal == 0)
        {
            return false;
        }
        else
        {
            target = targetsHit[0];
            return true;
        }

        //NavMeshPath nmp = new NavMeshPath();
        //agent.CalculatePath(new Vector3(), nmp);
        
        // Use the corners to calculate the distance
    }


    // I need the clown to see the player
    // In order to attack the player there needs to be a sightline + distance requirement.

    // what i can try is...

    // Check raw distance, if we're close enough start chasing
    // Approach until (can see player && within throwing distance)
}
