using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Rigidbody
    private new Rigidbody rigidbody;
    private bool HasRB = false;

    // Input actions
    private InputSystem_Actions actions;
    private InputAction move;
    private InputAction look;

    // Movement vars
    private Vector3 velVect = Vector3.zero;
    [SerializeField] private float SmoothTime;
    [SerializeField] private Vector2 targetVelocity;

    private void Awake()
    {
        actions = new InputSystem_Actions();

        if (!TryGetComponent(out rigidbody)) Debug.Log("The PlayerController component could not find a RigidBody");
        else HasRB = true;
    }

    private void OnEnable()
    {
        move = actions.Player.Move;
        move.Enable();

        look = actions.Player.Look;
        look.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        look.Disable();
    }

    private void Move()
    {
        // Vector2 (x,y) : x=AD y=WS
        Vector2 movement = move.ReadValue<Vector2>();

        // You probably don't need two of these but may as well when dealing with references.
        float speed_ref_x = 0;
        float speed_ref_z = 0;

        // These are the horizontal and vertical fractional speed. They range from 0 to 1. They will be multiplied by top speeds in each direction.
        // Strafe Speed
        velVect.x = Mathf.SmoothDamp(velVect.x, movement.x, ref speed_ref_x, SmoothTime * Time.fixedDeltaTime);
        // Forward Speed
        velVect.z = Mathf.SmoothDamp(velVect.z, movement.y, ref speed_ref_z, SmoothTime * Time.fixedDeltaTime);
        // No change to the y velocity in here.
        velVect.y = rigidbody.linearVelocity.y;

        // Figure out how to normalize this properly later.
        if (HasRB)
        {
            // The goal of this code is to move in the direction based on the forward vector.
            // To be precise, Rotate() changes the forward vector based on Player.Look input. Here, we want the Player.Move input to move the player based on the forward vector.
            // If we move backwards, then we move in the Vector3.backwards direction. If we move right and forward, we move at the sum of that angle.
            float directionHori = (velVect.x < -0.008) ? -1f : (velVect.x > 0.008) ? 1f: 0;
            float directionVert = (velVect.z < -0.008) ? -1f : (velVect.z > 0.008) ? 1f: 0;

            // This is not how you do this vector.
            // Take the forward vector and rotate it based on the player input.
            // If it's going forward keep it the same, backwards flip it.
            // If it's going right keep it the same, backwards flip it.
            
            Vector3 vertVect = (directionVert == 1) ? transform.forward: (directionVert == -1) ? -1f * transform.forward: Vector3.zero;
            Vector3 horiVect = (directionHori == 1) ? new Vector3(transform.forward.z,transform.forward.y,-transform.forward.x) : (directionHori == -1) ? new Vector3(-transform.forward.z, transform.forward.y, transform.forward.x) : Vector3.zero;

            Vector3 constructedForwardVect = vertVect + horiVect;

            // This one has to flip 180 when you go backwards
            // forwardVect.x = (directionHori == -1) ? forwardVect.x * -1: forwardVect.x;

            // This one has to flip 90 when right and 270 when left
            // forwardVect.z = (directionVert == -1) ? forwardVect.z * -1 : forwardVect.z;

            // If you want to flip 180 degrees, you need to flip both components.
            // If you want to flip 90 degrees, you need to swap the components bruh.

            // If you press S, you make both components negative.
            // If you press


            Debug.Log("forwardVect: " + constructedForwardVect);
            Debug.Log("transform.forwards: " + transform.forward);
            Debug.Log("direction vals: " + directionHori + " " + directionVert);
            Debug.Log("velVect vals " + velVect.x + " " + velVect.z);

            //Calculate angle between the velocity vector and the created forward vector.
            float theta = Mathf.Atan2(velVect.x * constructedForwardVect.z - velVect.z * constructedForwardVect.x, velVect.x * constructedForwardVect.x + velVect.z * constructedForwardVect.z);
            float[,] angleMatrix =
            {
                { Mathf.Cos(theta),-Mathf.Sin(theta) },
                { Mathf.Sin(theta), Mathf.Cos(theta) }
            };

            // Pseudo code: velVect = angleMatrix * velVect;
            Vector3 rotatedVel = Vector3.zero;
            rotatedVel.x = angleMatrix[0, 0] * velVect.x + angleMatrix[0, 1] * velVect.z;
            rotatedVel.z = angleMatrix[1, 0] * velVect.x + angleMatrix[1, 1] * velVect.z;
            rotatedVel.y = velVect.y;

            rigidbody.linearVelocity = rotatedVel;
        }
    }

    private void Rotate()
    {
        // Get the rotation vector
        Vector2 looking = look.ReadValue<Vector2>();
        //Debug.Log(looking);
        //Debug.Log(transform.forward);

        // Use that vector to rotate the player
        // only use the x component I guess

        rigidbody.transform.Rotate(0, looking.x, 0);
    }

    private void FixedUpdate()
    {
        if (HasRB)
        {
            Rotate();
            Move();
        }
    }
}
