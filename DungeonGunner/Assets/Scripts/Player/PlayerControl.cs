using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The player WeaponShootPosition gameObject in the hierarchy")]
    #endregion Tooltip

    [SerializeField] private Transform weaponShootPosition;
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private float moveSpeed;
    

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        //Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        //Create movement direction vector
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if(horizontalMovement != 0 && verticalMovement != 0)
        {
            //Normalize diagonal movement
            direction.Normalize();
            //direction *= 0.7f;
        }
        Debug.Log("Direction: " + direction);
        if (direction != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }

    }

    /// <summary>
    /// Player weapon input
    /// </summary>
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        //angle between cursor and player transform (pivot point)
        float playerAngleDegrees;
        //angle between cursor and weapon shoot position
        float weaponAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out playerAngleDegrees, out weaponAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float playerAngleDegrees, out float weaponAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        //calculate direction vector of mouse cursor from weapon shoot position
        weaponDirection = mouseWorldPosition - weaponShootPosition.position;

        //calculate direction vector of mouse cursor from player transform position
        Vector3 playerDirection = mouseWorldPosition - player.transform.position;

        //weapon to cursor angle
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //player to cursor angle
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        //set player aim direction
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallWeaponAimEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
