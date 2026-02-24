using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;


[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{

    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void Update()
    {
        if(isPlayerRolling)
        {
            // If the player is rolling, we don't want to process any other input
            return;
        }
        //process player input
        MovementInput();
        //process weapon input
        WeaponInput();

        //player roll cooldown timer
        PlayerRollCooldownTimer();
    }

    /// <summary>
    /// Player movement input
    /// </summary>
    private void MovementInput()
    {
        //Get movement input
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rollInput = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1);

        //Create movement direction vector
        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if(horizontalMovement != 0 && verticalMovement != 0)
        {
            //Normalize diagonal movement
            direction.Normalize();
            //direction *= 0.7f;
        }
        //Debug.Log("Direction: " + direction);

        //if there is movement or roll input
        if (direction != Vector2.zero)
        {
            if (!rollInput)
            {
                //trigger normal movement
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            else if(playerRollCooldownTimer <= 0f) //check if roll is on cooldown
            {
                PlayerRoll((Vector3)direction);
            }
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }

    }

    private void SetStartingWeapon()
    {
        int index = 1;
        foreach(Weapon weapon in player.weaponList)
        {
            if(weapon.weaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        //Set animator speed based on player move speed
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollCoroutine(direction));
    }

    private IEnumerator PlayerRollCoroutine(Vector3 direction)
    {
        //exit coroutine when minimum distance between target position and current position is reached
        float minDistance = 0.2f;
        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (direction * movementDetails.rollDistance);

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            //move player towards target position
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, true);
            yield return waitForFixedUpdate;
        }
        isPlayerRolling = false;
        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }

    private void PlayerRollCooldownTimer()
    {
        if (playerRollCooldownTimer > 0f)
        {
            playerRollCooldownTimer -= Time.deltaTime;
            if (playerRollCooldownTimer < 0f)
            {
                playerRollCooldownTimer = 0f;
            }
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

        //Fire Weapon Input
        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        //Switch Weapon Input
        SwitchWeaponInput();

        //Reload Weapon Input
        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float playerAngleDegrees, out float weaponAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        //calculate direction vector of mouse cursor from weapon shoot position
        weaponDirection = mouseWorldPosition - player.activeWeapon.GetWeaponShootPosition();

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

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        //left click to fire weapon
        if (Input.GetMouseButton(0))
        {
            //trigger fire weapon event
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame,playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput()
    {
        if(Input.mouseScrollDelta.y < 0f)
        {
            NextWeapon();
        }
        else if(Input.mouseScrollDelta.y >  0f)
        {
            PreviousWeapon();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeaponByIndex(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeaponByIndex(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeaponByIndex(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetWeaponByIndex(4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetWeaponByIndex(5);
        }

        #region Weapon 5 - 10
        
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetWeaponByIndex(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetWeaponByIndex(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetWeaponByIndex(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetWeaponByIndex(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SetWeaponByIndex(9);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetWeaponByIndex(10);
        }
        #endregion

        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetWeaponToFirstInList();
        }
    }
 
    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        //check if weapon can be reloaded
        //if weapon is currently reloading, return
        if(currentWeapon.isWeaponReloading)
            return;
        //if remaining ammo is less than clip capacity and weapon has no infinite ammo
        if (currentWeapon.weaponTotalRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo)
            return;
        //if clip ammo is full, return
        if(currentWeapon.weaponClipAmmoRemaining >= currentWeapon.weaponDetails.weaponClipAmmoCapacity)
            return;

        if(Input.GetKeyDown(KeyCode.R))
        {
            //trigger reload weapon event
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        if(weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;
        if(currentWeaponIndex > player.weaponList.Count)
        {
            currentWeaponIndex = 1;
        }
        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;
        if(currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.weaponList.Count;
        }
        SetWeaponByIndex(currentWeaponIndex);
    }

    /// <summary>
    /// For reordering of weapons in the weapon list, setting the current weapon to the first position in the list
    /// </summary>
    private void SetWeaponToFirstInList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        int index = 2;

        foreach(Weapon weapon in player.weaponList)
        {
            if(weapon != currentWeapon)
            {
                weapon.weaponListPosition = index;
                tempWeaponList.Add(weapon);
                index++;
            }
        }

        player.weaponList = tempWeaponList;

        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Stop player roll coroutine when colliding with an wall
        StopPlayerRollCoroutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Stop player roll coroutine when colliding with an wall
        StopPlayerRollCoroutine();
    }

    private void StopPlayerRollCoroutine()
    {
        if(playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);
            isPlayerRolling = false;
        }
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
