using UnityEngine;
using System;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponEventArgs() { fire = fire, aimDirection = aimDirection, 
            aimAngle = aimAngle, weaponAimAngle = weaponAimAngle, weaponAimDirectionVector = weaponAimDirectionVector });
    }
}

public class FireWeaponEventArgs : EventArgs
{
    public bool fire;
    public AimDirection aimDirection;
    //from the base of the character to the aim point
    public float aimAngle;
    //from the weapon shoot position to the aim point
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
