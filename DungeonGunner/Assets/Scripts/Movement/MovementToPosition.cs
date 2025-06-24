using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    Rigidbody2D rigidBody2D;
    MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidBody(movementToPositionArgs.movePosition, 
            movementToPositionArgs.currentPosition, 
            movementToPositionArgs.moveSpeed, 
            movementToPositionArgs.moveDirection, 
            movementToPositionArgs.isRolling);
    }

    private void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed, Vector2 moveDirection, bool isRolling)
    {
        // Calculate the direction to move towards the target position
        Vector2 direction = (movePosition - currentPosition).normalized;
        
        rigidBody2D.MovePosition((direction * moveSpeed * Time.fixedDeltaTime) + rigidBody2D.position);
    }
}
