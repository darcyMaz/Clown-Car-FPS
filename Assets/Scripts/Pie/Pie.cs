using System;
using UnityEngine;

public class Pie : MonoBehaviour
{
    // Two states
    // In hand = 0
    // Thrown = 1
    // Target Reached / Falling = 2
    private int State = 0;

    private Transform Hand;
    private bool IsHandSet = false;

    private Vector3 Target;
    private bool IsTargetSet = false;

    private Rigidbody rigidBody;

    private EnemyMovement em;
    private bool IsEMSet = false;

    [SerializeField] private float Speed = 3f;
    [SerializeField] private int Damage = 2;

    private void Awake()
    {
        if (!TryGetComponent(out rigidBody))
        {
            Debug.Log("A Pie was instantiated but had no RigidBody. It was destroyed.");
            Destroy(gameObject);
        }

        rigidBody.useGravity = false;
    }

    private void OnDisable()
    {
        if (IsEMSet) em.OnPieThrown -= PieThrown;
    }

    private void FixedUpdate()
    {
        // if state = 0 then transform.position = Hand
        // else if state = 1  move towards target
        // else fall

        // if in hand, always face target.
        // else if thrown, face last forward direction of in hand
        // else rotate as it falls

        if (State == 0) InHand();
        else if (State == 1) Throwing();
        else if (State == 2) Falling();
        else
        {
            Debug.Log("A pie was thrown but the state has an improper value. The pie will be destroyed.");
            Splat(null);
        }

    }


    private void InHand()
    {
        // Stick pie to the hand
        if (IsHandSet && Hand != null) transform.position = Hand.position;
        else { State = 2; rigidBody.useGravity = true; }

        // Look at the player but not its y component. I.e. don't tilt the pie down at the player while it's in the hand.
        rigidBody.transform.LookAt(new Vector3(Target.x, transform.position.y, Target.z));
    }

    private void Throwing()
    {
        rigidBody.MovePosition(Vector3.MoveTowards(rigidBody.transform.position, Target, Speed));
        // transform.position = Vector3.MoveTowards(rigidBody.transform.position, Target, 10);

        // If we reach the destination without hitting anything, start falling.
        if ((Target - transform.position).magnitude < 0.1)
        {
            State = 2;
            rigidBody.useGravity = true;
        }
    }

    private void Falling()
    {
        // Fall endlessly.
        // Despawn if it's going under the map too far.

        

        if (transform.position.y < -400)
        {
            Splat(null);
        }
    }

    private void Splat(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Player")
            {
                // try get player health
                // decrement health
                PlayerHealth ph;

                if (other.gameObject.TryGetComponent(out ph))
                {
                    ph.ChangeHealth(-Damage);
                }
            }
            else { Debug.Log("Hit not player!"); }
        }
        Destroy(gameObject);

        // Play a noise, particle effect, etc
    }

    public void PieThrown()
    {
        State = 1;
    }
    public void SetHand(Transform hand)
    {
        if (!IsHandSet)
        {
            Hand = hand;
            IsHandSet = true;
        }
    }
    public void SetTarget(Vector3 target)
    {
        if (!IsTargetSet)
        {
            Target = target;
            IsTargetSet = true;
        }
    }
    public void SetEnemyMovement(EnemyMovement eM)
    {
        if (IsEMSet) return;
        IsEMSet = true;
        em = eM;
        em.OnPieThrown += PieThrown;
    }


    private void OnTriggerEnter(Collider other)
    {
        // Make this a trigger
        
        // Ideally, any collision would result in the pie splatting.
        // Buuut, I couldn't figure out how to make the clown keep a healthy distance from a wall once they see the player and starting throwing.
        // My vectors didn't work lol, I don't have enough time to get them to work.
        // So, it'll only splat when it hits the player.
        // Ok it'll splat on the floor too.

        if (other.tag == "Player" || other.tag == "Floor")
        {
            Splat(other);
        }
        
    }
}
