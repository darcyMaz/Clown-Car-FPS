using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Attack is an event
    // On click, create a ray between player and whatever is in direction of pointer in the middle of the screen...
    // idk how to do that lol
    // Gotta figure it out

    // The Raycast docs literally have an example of this as its example so I'll use that.
    // start: camera pos + offset maybe, direction is forward vector, distance is some set distance, create layer mask for enemies and whatever

    [SerializeField] private Transform CameraTransform;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float MaxShootingDistance;

    private void FixedUpdate()
    {

        // So this works! Returns true when a raycast pointing at an enemy
        bool didHit = Physics.Raycast(CameraTransform.position, transform.forward, 40f, enemyMask);
        Debug.Log(didHit);
    }


}
