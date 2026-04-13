using UnityEngine;
using UnityEngine.AI;

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
    private Animator animator;
    private bool HasAnimator = false;

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

        if (!TryGetComponent(out animator)) Debug.Log("An EnemyMovement Script could not find its corresponding Animator component.");
        else HasAnimator = true;
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
            bool foundTarget = FindTarget();
            Debug.Log("Found target: " + foundTarget + " isAttacking: " + IsAttacking);

            // So this AI does search for players every frame.
            // This won't be super inefficient but maybe there's a better way.
            // I don't think so tho lol.
            if (foundTarget && !IsAttacking)
            {
                

                // Go towards target.
                //agent.path = TargetPath;
                //agent.isStopped = false;
                agent.SetPath(TargetPath);
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
        Collider[] targetsHitTemp = new Collider[MaxPlayersInGame];
        int hitTotal = Physics.OverlapSphereNonAlloc(transform.position, CircleSearch, targetsHitTemp, PlayerLayer);

        // Move targets hit to an array of an exact size.
        Collider[] targetsHit = new Collider[hitTotal];
        for (int i=0;i<hitTotal;i++)
        {
            targetsHit[i] = targetsHitTemp[i];
        }

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

            for (int targetIndex = 0; targetIndex < hitTotal; targetIndex++)
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
            Debug.Log("Try attack too far");
            return false;
        }

        // If the enemy can see the player / the view of the player is not obscured.
        //      Think about putting on offset in the y position for this
        if (!Physics.Raycast
            (
                transform.position, 
                new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z),
                AttackDistance * 2,
                PlayerLayer
            )
           )
        {
            Debug.Log("Try attack can't see player");
            return false;
        }

        // Stop moving towards the path.
        agent.isStopped = true;
        // Look at the player
        transform.LookAt(target.transform);
        // Trigger the pie throw.
        if (HasAnimator) animator.SetTrigger("ThrowPie");
        IsAttacking = true;

        Debug.Log("Try attack true");

        return true;
    }

    public void AttackDone()
    {
        IsAttacking = false;
    }
}
