using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCooldownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    private void Update()
    {
        if(fireRateCooldownTimer > 0f)
            fireRateCooldownTimer -= Time.deltaTime;
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponPreCharge(fireWeaponEventArgs);

        if (fireWeaponEventArgs.fire)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCooldownTimer();
                ResetPreChargeTimer();
            }
        }
    }

    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPreChargeTimer();
        }
    }

    private bool IsWeaponReadyToFire()
    {
        //check if there is ammo in the weapon (overall)
        if(activeWeapon.GetCurrentWeapon().weaponTotalRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;
        //check if wapon is reloading
        if(activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;
        //check if fire rate cooldown timer is finished or if weap[on has precharge time remaining
        if (fireRateCooldownTimer > 0f || firePreChargeTimer > 0f)
            return false;
        //check if there is ammo in the weapon (in clip) and weapon does not have infinite clip capacity, reload the weapon instead of firing if there is no ammo
        if (activeWeapon.GetCurrentWeapon().weaponClipAmmoRemaining <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            //trigger reload weapon event
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }

        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetAmmoDetails();

        if(currentAmmo != null)
        {
            //get random ammo prefab from array
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            //get random ammo speed
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            //Get gameobject from object pool
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetWeaponShootPosition(), Quaternion.identity);

            //Initialize ammo
            ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            //reduce ammo clip count if not infinite 
            if(!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
            {
                activeWeapon.GetCurrentWeapon().weaponClipAmmoRemaining--;
                activeWeapon.GetCurrentWeapon().weaponTotalRemainingAmmo--;
            }

            //call weapon fired event
            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        }
    }

    private void ResetCooldownTimer()
    {
        fireRateCooldownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void ResetPreChargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }
}
