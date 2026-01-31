using UnityEngine;
using System;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    public void CallReloadWeaponEvent(Weapon weapon, int topUpAmmoPercent)
    {
        OnReloadWeapon?.Invoke(this, new ReloadWeaponEventArgs() { weapon = weapon, topUpAmmoPercent = topUpAmmoPercent });
    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon weapon;
    //how many % of ammo to top up (top up = add to total ammo capacity)
    public int topUpAmmoPercent;
}
