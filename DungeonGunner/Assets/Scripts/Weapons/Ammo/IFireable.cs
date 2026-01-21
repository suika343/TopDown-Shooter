using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    //all methods in an interface are public by default
    void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, 
        Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false);

    GameObject GetGameObject();
}
