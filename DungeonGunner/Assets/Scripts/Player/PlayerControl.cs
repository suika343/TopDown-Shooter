using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("The player WeaponShootPosition gameObject in the hierarchy")]
    #endregion Tooltip

    [SerializeField] private Transform weaponShootPosition;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    private void MovementInput()
    {
        player.idleEvent.CallIdleEvent();
    }

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
}
