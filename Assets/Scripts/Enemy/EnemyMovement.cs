using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    // Navmesh Agent
    private NavMeshAgent agent;
    private bool HasAgent = false;

    // Search vars
    [SerializeField] private float CircleSearch = 50f;
    [SerializeField] private float AttackDistance;
    [SerializeField] private LayerMask PlayerLayer;
    // Target vars
    private Collider target;
    private float TargetDistance = float.MaxValue;
    private NavMeshPath TargetPath;

    // Animation vars
    private bool IsAttacking = false;
    private Animator animator;
    private bool HasAnimator = false;
    [SerializeField] private float RestTime = 2f;
    private float RestTimer = 0f;

    // Misc vars
    [SerializeField] private int MaxPlayersInGame = 12;

    public event Action <Vector3> OnAttack;
    public event Action OnPieThrown;

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
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log(transform.position);

        if (HasAgent)
        {
            if (RestTimer > 0)
            {
                RestTimer -= Time.fixedDeltaTime;
                return;
            }
            else
            {
                RestTimer = 0;
                animator.SetTrigger("RestDone");
            }

            bool foundTarget = FindTarget();
            //Debug.Log("Found target: " + foundTarget + " isAttacking: " + IsAttacking);

            // So this AI does search for players every frame.
            // This won't be super inefficient but maybe there's a better way.
            // I don't think so tho lol.
            if (foundTarget && !IsAttacking)
            {
                animator.SetBool("isRunning", true);

                // Go towards target.
                agent.SetPath(TargetPath);

                // Try to attack target, check if we meet requirements to.
                TryAttack();
            }
            else
            {
                animator.SetBool("isRunning", false);
                agent.ResetPath();

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
            //Debug.Log("Try attack too far");
            return false;
        }

        RaycastHit hit_temp;
        // If the enemy can see anything between it and the target.
        if (
            Physics.Raycast
            (
                transform.position,
                new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z),
                out hit_temp, 
                AttackDistance,
                ~(0) - (PlayerLayer) // Well now I know how this stuff works lol
             )
           )
        {
            //Debug.Log("Attack failed, enemy obstructed: " + hit_temp.transform.name + " LayerMask: " + Convert.ToString(~(0) - (1 << PlayerLayer), 2));
            return false;
        }


        Vector3 basicSightline = new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z);
        
        /*
        Vector3 bsl_270 = new Vector3(basicSightline.z, basicSightline.y, -basicSightline.x);
        Vector3 bsl_90 = new Vector3(-basicSightline.z, basicSightline.y, basicSightline.x);

        Vector3[] SixSightLines = new Vector3[2];
        SixSightLines[0] = new Vector3(target.transform.position.x - bsl_90.normalized.x, target.transform.position.y - bsl_90.normalized.y, target.transform.position.z - bsl_90.normalized.z);
        SixSightLines[1] = new Vector3(target.transform.position.x - bsl_270.normalized.x, target.transform.position.y - bsl_270.normalized.y, target.transform.position.z - bsl_270.normalized.z);


        // BSL
        // BSL_90 normalized
        // Make 6 vectors:
        //      The same one and one mirror flipped
        //      2 half way up the character
        //      2 more at the top of the character
        // If all 6 vectors can see character then throw pie

        
        Debug.DrawRay(transform.position, basicSightline, Color.orange);
        Debug.DrawRay(transform.position, bsl_90, Color.green);
        Debug.DrawRay(transform.position, bsl_270, Color.green);

        Debug.DrawRay(bsl_90.normalized - transform.position, target.transform.position - (bsl_90.normalized - transform.position), Color.red);
        Debug.DrawRay(bsl_270.normalized - transform.position, target.transform.position - (bsl_270.normalized - transform.position), Color.red);

        //Debug.DrawRay(transform.position, DistanceVect2, Color.blue, AttackDistance * 2);
        //Debug.DrawRay(transform.position, DistanceVect1, Color.red, AttackDistance * 2);

        
        if (!(Physics.Raycast(transform.position, SixSightLines[0], PlayerLayer) && Physics.Raycast(transform.position, SixSightLines[1], PlayerLayer)))
        {
            Debug.Log("enemy was partially or completely obscured");
            return false;
        }
        */



        // Is the player within the AttackDistance?
        if (
             
            !Physics.Raycast
            (
                transform.position, 
                new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z),
                AttackDistance,
                PlayerLayer
            )
            
           )
        {
            //Debug.Log("Try attack can't see player");
            //Debug.DrawRay(
             //   transform.position,
            //    new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z),
            //    Color.red
            //    );
            return false;
        }
        

        // If the enemy is not looking right at the player.
        if (!Physics.Raycast(transform.position, transform.forward, AttackDistance, PlayerLayer))
        {
            //Debug.Log("Try attack false, not looking right at the player.");
            //Debug.DrawRay(transform.position, transform.forward, Color.green, AttackDistance);

            float theta = Mathf.Atan2(transform.forward.x * basicSightline.z - transform.forward.z * basicSightline.x, transform.forward.x * basicSightline.x + transform.forward.z * basicSightline.z);
            float rotate_dir = (theta > 0) ? -1 : 1;

            transform.Rotate(0, rotate_dir * 5f, 0); // Calc whether to go clockwise or counter clockwise

            return false;
        }

        Attack();

        //Debug.Log("Try attack true");

        return true;
    }

    private void Attack()
    {
        if (HasAnimator) animator.SetTrigger("Attack");
        IsAttacking = true;
        OnAttack?.Invoke(target.transform.position);
    }

    public void AttackDone()
    {
        if (HasAnimator) animator.SetTrigger("AttackDone");
        IsAttacking = false;
        RestTimer = RestTime;
    }

    public void ThrowPie()
    {
        OnPieThrown?.Invoke();
    }
}
