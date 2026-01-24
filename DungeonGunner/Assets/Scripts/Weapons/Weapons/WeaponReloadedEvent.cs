using UnityEngine;
using System;

[DisallowMultipleComponent]
public class WeaponReloadedEvent : MonoBehaviour
{
    public event Action<WeaponReloadedEvent, WeaponReloadedEventArgs> OnWeaponReloaded;

    public void CallOnWeaponReloadedEvent(Weapon weapon)
    {
        OnWeaponReloaded?.Invoke(this, new WeaponReloadedEventArgs { weapon = weapon });
    }
}

public class WeaponReloadedEventArgs : EventArgs
{
    public Weapon weapon;
}
