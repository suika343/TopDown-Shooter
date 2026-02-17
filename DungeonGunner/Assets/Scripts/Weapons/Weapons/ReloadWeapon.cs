using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]

[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;

    private Coroutine reloadWeaponCoroutine;

    private void Awake()
    {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeaponEvent;

        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeaponEvent;

        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void ReloadWeaponEvent_OnReloadWeaponEvent(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if(reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.topUpAmmoPercent));
    }

    private IEnumerator ReloadWeaponCoroutine(Weapon weapon, int topUpAmmoPercent)
    {
        //set weapon as reloading
        weapon.isWeaponReloading = true;

        if(weapon.weaponDetails.weaponReloadingSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.weaponReloadingSoundEffect);
        }

        //Update reload timer
        while (weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        //if total ammo is to be increased
        if(topUpAmmoPercent != 0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.weaponAmmoCapacity * topUpAmmoPercent) / 100f);

            int totalAmmo = weapon.weaponTotalRemainingAmmo + ammoIncrease;

            if(totalAmmo > weapon.weaponDetails.weaponAmmoCapacity)
            {
                weapon.weaponTotalRemainingAmmo = weapon.weaponDetails.weaponAmmoCapacity;
            }
            else
            {
                weapon.weaponTotalRemainingAmmo = totalAmmo;
            }
        }

        //if weapon has infinite ammo, then just refill the clip
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.weaponClipAmmoRemaining = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        // else if not infinite ammo then if remaining ammo is greater than the amount required to refill the clip, then fully refill the clip
        else if (weapon.weaponTotalRemainingAmmo >= weapon.weaponDetails.weaponClipAmmoCapacity)
        {
            weapon.weaponClipAmmoRemaining = weapon.weaponDetails.weaponClipAmmoCapacity;
        }
        else
        {
            weapon.weaponClipAmmoRemaining = weapon.weaponTotalRemainingAmmo;
        }

        //reset weapon reload timer
        weapon.weaponReloadTimer = 0f;

        //set weapon as not reloading
        weapon.isWeaponReloading = false;

        //Call weapon reloaded event
        weaponReloadedEvent.CallOnWeaponReloadedEvent(weapon);
    }


    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if(setActiveWeaponEventArgs.weapon.isWeaponReloading)
        {
            if(reloadWeaponCoroutine != null)
            {
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponCoroutine(setActiveWeaponEventArgs.weapon, 0));
        }
    }
}
