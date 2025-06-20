using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{

    private Rigidbody2D rigidBody2D;
    private MovementByVelocityEvent movementByVelocityEvent;
    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidbody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    /// <summary>
    /// Moves the Rigidbody2D in the specified direction with the specified speed.
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="moveSpeed"></param>
    private void MoveRigidbody (Vector2 moveDirection, float moveSpeed)
    {
        // Ensure the Rigidbody2D is collision detection is set to Continuous 
        // TIme.deltaTime is not necessary for rigidbody2D movement as it uses fixed physics updates.
        GetComponent<Rigidbody2D>().linearVelocity = moveDirection * moveSpeed;
    }
}
