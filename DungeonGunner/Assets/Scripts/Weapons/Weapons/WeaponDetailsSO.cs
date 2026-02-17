using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region HEADER Weapon Base Details
    [Space(10)]
    [Header("Weapon BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Weapon Name")]
    #endregion
    public string weaponName;
    #region Tooltip
    [Tooltip("The sprite for the weapon - the sprite should have the 'generate physics shape' option selected")]
    #endregion
    public Sprite weaponSprite;

    #region HEADER Weapon Configuration
    [Space(10)]
    [Header("Weapon Configration")]
    #endregion
    #region Tooltip
    [Tooltip("Weapon Shoot Position - the offset postion for the end of the weapon from the sprite pivot point")]
    #endregion
    public Vector3 weaponShootPosition;
    #region Tooltip
    [Tooltip("Weapon current ammo")]
    #endregion    
    public AmmoDetailsSO weaponCurrentAmmo;

    #region Tooltip
    [Tooltip("Weapon firing sound effect")]
    #endregion    
    public SoundEffectSO weaponFiringSoundEffect;

    #region Tooltip
    [Tooltip("Weapon reloading sound effect")]
    #endregion    
    public SoundEffectSO weaponReloadingSoundEffect;

    #region HEADER Weapon Operating Values
    [Space(10)]
    [Header("Weapon Operating Values")]
    #endregion
    #region Tooltip
    [Tooltip("Select if weapon has infinite ammo")]
    #endregion
    public bool hasInfiniteAmmo = false;
    #region Tooltip
    [Tooltip("Select if weapon has infinite clip capacity")]
    #endregion
    public bool hasInfiniteClipCapacity = false;
    #region Tooltip
    [Tooltip("Number of shots before a reload")]
    #endregion
    public int weaponClipAmmoCapacity = 6;
    #region Tooltip
    [Tooltip("Weapon ammo capacity - the maximum number of rounds that can be held for this weapon")]
    #endregion
    public int weaponAmmoCapacity = 100;
    #region Tooltip
    [Tooltip("Weapon fire rate - '0.2 means 5 shots a second'")]
    #endregion
    public float weaponFireRate = 0.2f;
    #region Tooltip
    [Tooltip("Weapon Precharge time - time in seconds to hold the fire button down before firing")]
    #endregion
    public float weaponPrechargeTime = 0f;
    #region Tooltip
    [Tooltip("Weapon Reload time - Reload time in seconds")]
    #endregion
    public float weaponReloadTime = 0f;

    #region VALIDTION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSprite), weaponSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);

        if(!hasInfiniteClipCapacity)
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
    }
#endif
    #endregion
}
