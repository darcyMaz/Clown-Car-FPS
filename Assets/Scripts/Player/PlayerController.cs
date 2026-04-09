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

        // Values super close to zero will be set to zero.
        // velVect.x = (velVect.x < 0.001 && movement.x == 0) ? 0 : velVect.x;
        // velVect.z = (velVect.z < 0.001 && movement.y == 0) ? 0 : velVect.z;

        // Figure out how to normalize this properly later.
        if (HasRB)
        {
            // The goal of this code is to move in the direction based on the forward vector.
            // To be precise, Rotate() changes the forward vector based on Player.Look input. Here, we want the Player.Move input to move the player based on the forward vector.
            // If we move backwards, then we move in the Vector3.backwards direction. If we move right and forward, we move at the sum of that angle.
            float directionHori = (velVect.x < -0.005) ? -1 : (velVect.x > 0.005) ? 1: 0;
            float directionVert = (velVect.z < -0.005) ? -1 : (velVect.z > 0.005) ? 1: 0;
            // Change these such that: less than 0.01 and greater than 0.01 results in zeroh


            // I'm sure this is the problem right?
            Vector3 forwardVect = transform.forward;
            forwardVect.x *= directionHori;
            forwardVect.z *= directionVert;
            forwardVect = forwardVect.normalized;

            // velvect.x and z can't be negative for some reason???
            Debug.Log("forwardVect: " + forwardVect);
            Debug.Log("transform.forwards: " + transform.forward);
            Debug.Log("direction vals: " + directionHori + " " + directionVert);
            Debug.Log("velVect vals " + velVect.x + " " + velVect.z);

            //Calculate angle between the velocity vector and the created forward vector.
            float theta = Mathf.Atan2(velVect.x * forwardVect.z - velVect.z * forwardVect.x, velVect.x * forwardVect.x + velVect.z * forwardVect.z);
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

            

            /*
            Vector3 linearVelocity = new Vector3(velVect.x * targetVelocity.x, rigidbody.linearVelocity.y, velVect.z * targetVelocity.y);

            // 1) Calculate angle between the linearVelocity vector and the forward vector.
            // 2) Rotate the linearVelocity vector by that angle.

            float theta = Mathf.Atan2(linearVelocity.x * transform.forward.z - linearVelocity.z * transform.forward.x, linearVelocity.x * transform.forward.x + linearVelocity.z * transform.forward.z);
            float[,] angleMatrix = 
            { 
                { Mathf.Cos(theta),-Mathf.Sin(theta) },
                { Mathf.Sin(theta), Mathf.Cos(theta) } 
            };

            // Pseudo code: linearVelocity = angleMatrix * linearVelocity;
            Vector3 rotatedLinVel = Vector3.zero;
            rotatedLinVel.x = angleMatrix[0, 0] * linearVelocity.x + angleMatrix[0, 1] * linearVelocity.z;
            rotatedLinVel.z = angleMatrix[1, 0] * linearVelocity.x + angleMatrix[1, 1] * linearVelocity.z;
            rotatedLinVel.y = linearVelocity.y;

            Debug.Log(rotatedLinVel + " " + linearVelocity);

            rigidbody.linearVelocity = rotatedLinVel;
            */
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
