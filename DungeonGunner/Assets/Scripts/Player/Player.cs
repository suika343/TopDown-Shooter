using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeapon))]
[DisallowMultipleComponent]
#endregion
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public ReloadWeapon reloadWeapon;

    public List<Weapon> weaponList = new List<Weapon>();
    private void Awake()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        idleEvent = GetComponent<IdleEvent>(); 
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        reloadWeapon = GetComponent<ReloadWeapon>();
    }

    /// <summary>
    /// Initialize player with player details
    /// </summary>
    /// <param name="playerDetails"></param>
    public void InitializePlayer(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        CreatePlayerStartingWeapons();

        //Set player Health
        SetPlayerHealth();
    }

    /// <summary>
    /// Set player health
    /// </summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    private void CreatePlayerStartingWeapons()
    {
        weaponList.Clear();

        foreach(WeaponDetailsSO weaponDetails in playerDetails.startingWeaponsList)
        {
            AddWeaponToPlayer(weaponDetails);
        }
    }

    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon()
        {
            weaponDetails = weaponDetails,
            weaponListPosition = 0,
            weaponReloadTimer = weaponDetails.weaponReloadTime,
            weaponClipAmmoRemaining = weaponDetails.weaponClipAmmoCapacity,
            weaponTotalRemainingAmmo = weaponDetails.weaponAmmoCapacity,
            isWeaponReloading = false
        };

        //add weapon to player weapon list
        weaponList.Add(weapon);

        //set weapon list position
        weapon.weaponListPosition = weaponList.Count;

        //Set added weapon as active weapon
        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }
}
