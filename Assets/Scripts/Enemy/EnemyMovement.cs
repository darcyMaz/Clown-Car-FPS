using UnityEditor.ShaderGraph.Internal;
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

    private NavMeshAgent agent;
    private bool HasAgent = false;

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

    private void FixedUpdate()
    {
        FindTarget();
    }

    private void FindTarget()
    {


        NavMeshPath nmp = new NavMeshPath();
        agent.CalculatePath(new Vector3(), nmp);
    }

}
