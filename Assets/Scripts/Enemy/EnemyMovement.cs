using UnityEngine;
using UnityEngine.AI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class EnemyMovement : MonoBehaviour
{

    // Navmesh Agent
    private NavMeshAgent agent;
    private bool HasAgent = false;

    // Search vars
    [SerializeField] private float CircleSearch = 50f;
    private float AttackDistance = 0f;
    [SerializeField] private LayerMask PlayerLayer;
    // Target vars
    private Collider target;
    private float TargetDistance = float.MaxValue;
    private NavMeshPath TargetPath;

    // Animation vars
    private bool IsAttacking = false;

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
            AttackDistance = agent.stoppingDistance;

            FindTarget();
        }
    }

    private void FixedUpdate()
    {
        if (HasAgent)
        {
            if (FindTarget() && !IsAttacking)
            {
                // Go towards target.
                agent.path = TargetPath;
                agent.isStopped = false;

                // Try to attack target, check if we meet requirements to.
                TryAttack();
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    private bool FindTarget()
    {
        //Collider[] targetsHit = Physics.OverlapSphereNonAlloc(transform.position, CircleSearch, PlayerLayer);
        Collider[] targetsHit = new Collider[MaxPlayersInGame];
        int hitTotal = Physics.OverlapSphereNonAlloc(transform.position, CircleSearch, targetsHit);

        if (hitTotal == 0)
        {
            return false;
        }
        else if (hitTotal == 1)
        {
            // Set the only path to be the target path.

            NavMeshPath nmp = new NavMeshPath();
            agent.CalculatePath(targetsHit[0].transform.position, nmp);

            float distance = 0f;
            for (int pathIndex = 1; pathIndex < nmp.corners.Length; pathIndex++)
            {
                distance += Vector3.Distance(nmp.corners[pathIndex - 1], nmp.corners[pathIndex]);
            }

            target = targetsHit[0];
            TargetDistance = distance;
            TargetPath = nmp;

            return true;
        }
        else
        {
            // This code here selects the closest Collider found by the Sphere Search.
            // I am not certain whether these colliders are sorted by distance so I did this in case it is not.

            Collider closestCollider = targetsHit[0];
            float shortestDistance = float.MaxValue;

            for (int targetIndex = 0; targetIndex < targetsHit.Length; targetIndex++)
            {
                NavMeshPath nmp = new NavMeshPath();
                agent.CalculatePath(targetsHit[targetIndex].transform.position, nmp);

                float distance = 0f;
                for (int pathIndex = 1; pathIndex < nmp.corners.Length; pathIndex++)
                {
                    distance += Vector3.Distance(nmp.corners[pathIndex - 1], nmp.corners[pathIndex]);
                }

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestCollider = targetsHit[targetIndex];
                    TargetPath = nmp;
                }
            }

            return true;
        }
    }

    private bool TryAttack()
    {
        // Return false if the path to the target is too long.
        if (TargetDistance > AttackDistance)
        {
            return false;
        }

        // Return false if this enemy cannot see the player.
        //   At this stage, I have the player's collider. So, I can just make a ray cast directly at the player.
        //   False means something's in the way. True means we can see the player.
        //   False can also be a distance thing but that shouldn't matter.
        // bool CanSeePlayer = Physics.Raycast();

        // Perform the attack and then return true.
        //   Set animation bool to true.
        //   Spawn in a pie
        //   God do I need to code the pie so that it stays on the hand until thrown and then move towards where the player was? Yes lol.

        // Stop moving towards the path.
        agent.isStopped = true;

        return true;
    }

    // I need the clown to see the player
    // In order to attack the player there needs to be a sightline + distance requirement.

    // what i can try is...

    // Check raw distance, if we're close enough start chasing
    // Approach until (can see player && within throwing distance)
}
