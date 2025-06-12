using UnityEngine;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallWeaponAimEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
       OnWeaponAim?.Invoke(this, new AimWeaponEventArgs() {
           aimDirection = aimDirection,
           aimAngle = aimAngle,
           weaponAimAngle = weaponAimAngle,
           weaponAimDirectionVector = weaponAimDirectionVector
       });
    }
}

public class  AimWeaponEventArgs : EventArgs 
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
