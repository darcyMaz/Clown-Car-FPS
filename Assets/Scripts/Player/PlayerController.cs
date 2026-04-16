using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour
{
    
    // Rigidbody
    private new Rigidbody rigidbody;
    private bool HasRB = false;

    // Input actions
    private InputSystem_Actions actions;
    private InputAction move;
    private InputAction look;
    private InputAction attack;

    // Movement vars
    private Vector3 velVect = Vector3.zero;
    [SerializeField] private float SmoothTime = 5;
    [SerializeField] private Vector2 targetVelocity = new Vector2(6,10);

    // Attack vars
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float MaxShootingDistance;
    [SerializeField] private int ShootingDamage = 1;

    private void Awake()
    {
        // Locks the cursor into the center of the screen.
        Cursor.lockState = CursorLockMode.Locked;

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

        attack = actions.Player.Attack;
        attack.performed += Attack;
        attack.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
    }

    private void Move()
    {
        

        if (HasRB)
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

            // The goal of this code is to move in the direction based on the forward vector.
            // To be precise, Rotate() changes the forward vector based on Player.Look input. Here, we want the Player.Move input to move the player based on the forward vector.

            // We will construct a new Vector based on the input received.
            // Positive, negative, or zero direction forward and backwards.
            // So there can be 9 possible InputVectors.
            // Forward-Zero, Forward-Right, Forward-Left, Backwards-0, BR, BL, 00, 0R, 0L 
            float directionHori = (velVect.x < -0.1) ? -1f : (velVect.x > 0.1) ? 1f: 0;
            float directionVert = (velVect.z < -0.1) ? -1f : (velVect.z > 0.1) ? 1f: 0;

            // If going forward, vertVect is the forward vector, if backwards, it is the forwards vector fliped 180 degrees (the backwards vector?)
            Vector3 vertVect = (directionVert == 1) ? transform.forward: (directionVert == -1) ? -1f * transform.forward: Vector3.zero;
            // If going right, then horiVect is the forward vector rotated 90 degrees (right). If backwards, it is the forward vector rotated 270 degrees/-90 degrees (left).
            Vector3 horiVect = (directionHori == 1) ? new Vector3(transform.forward.z,transform.forward.y,-transform.forward.x) : (directionHori == -1) ? new Vector3(-transform.forward.z, transform.forward.y, transform.forward.x) : Vector3.zero;

            // The sum of these two vectors represents the InputVector, i.e. the forward vector warped to account for the input.
            Vector3 InputVector = vertVect + horiVect;

            //Calculate angle between the velocity vector and the created forward vector.
            float theta = Mathf.Atan2(velVect.x * InputVector.z - velVect.z * InputVector.x, velVect.x * InputVector.x + velVect.z * InputVector.z);
            float[,] angleMatrix =
            {
                { Mathf.Cos(theta),-Mathf.Sin(theta) },
                { Mathf.Sin(theta), Mathf.Cos(theta) }
            };

            // Pseudo code: rotatedVel = angleMatrix * velVect;
            Vector3 rotatedVel = Vector3.zero;
            rotatedVel.x = (angleMatrix[0, 0] * velVect.x + angleMatrix[0, 1] * velVect.z) * targetVelocity.x;
            rotatedVel.z = (angleMatrix[1, 0] * velVect.x + angleMatrix[1, 1] * velVect.z) * targetVelocity.y;
            rotatedVel.y = velVect.y;

            rigidbody.linearVelocity = rotatedVel;
        }
    }

    private void Rotate()
    {
        // Get the rotation vector
        Vector2 looking = look.ReadValue<Vector2>();
        
        // Clamp the vertical rotation / the rotation about the x-axis
        // To-do

        // Apply the rotation
        if (HasRB) rigidbody.transform.Rotate(0, looking.x, 0);
    }

    private void FixedUpdate()
    {
        if (HasRB)
        {
            Rotate();
            Move();
        }
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        

        if (context.performed)
        {
            //Debug.Log("Attack pressed");

            /*
             * 
             * GUN SHOT HERE
             * 
             */


            RaycastHit hit;
            bool didHit = Physics.Raycast(CameraTransform.position, transform.forward * MaxShootingDistance, out hit, MaxShootingDistance, enemyMask);


            Debug.DrawRay(CameraTransform.position, transform.forward * MaxShootingDistance, Color.green, 10);



            // I should probably replace this with a "Shootable Object" class. Whatever.
            if (didHit)
            {
                Debug.Log("Something was hit: " + hit.transform.name);

                EnemyHealth enemyHealth;
                if (!hit.transform.gameObject.TryGetComponent(out enemyHealth)) Debug.Log("A GameObject with the Enemy LayerMask was shot at but it does not have an EnemyHealth component");
                else
                {
                    //Debug.Log("Enemy Hit");

                    /*
                     * 
                     *  ENEMY HIT HERE
                     * 
                     * 
                     */


                    enemyHealth.ChangeHealth(ShootingDamage * -1);
                }
            }
        }
    }
}
