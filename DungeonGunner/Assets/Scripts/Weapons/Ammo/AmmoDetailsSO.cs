using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    #region Header BASIC AMMO DETAILS
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Name of the ammo")]
    #endregion
    public string ammoName;
    public bool isPlayerAmmo;

    #region Header AMMO SPRITE, PREFAB, and MATERIALS
    [Space(10)]
    [Header("AMMO SPRITE, PREFAB, and MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Sprite to be used for the ammo")]
    #endregion
    public Sprite ammoSprite;
    #region Tooltip
    [Tooltip("Populate with the prefab to be used for the ammo. If multiple prefabs are specified then a random prefab from the array will be selected." +
        "The prefab can be an ammo pattern - as long as it confroms to the IFireable interface")]
    #endregion
    public GameObject[] ammoPrefabArray;
    #region Tooltip
    [Tooltip("The material used for the ammo")]
    #endregion
    public Material ammoMaterial;
    #region Tooltip
    [Tooltip("If the ammo should 'charge' briefly before moving, set the time in seconds that the ammo is held charging after firing before release")]
    #endregion
    public float ammoChargeTime = 0.1f;
    #region Tooltip
    [Tooltip("If the ammo has charge time, then specify what material should be used to render the ammo while charging")]
    #endregion
    public Material ammoChargeMaterial;

    #region Header AMMO BASE PARAMETERS
    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    #endregion
    #region Tooltip
    [Tooltip("The damage each ammo deals")]
    #endregion
    public int ammoDamage = 1;
    #region Tooltip
    [Tooltip("The minimum speed of the ammo - the speed will be a random value between the min and max")]
    #endregion
    public float ammoSpeedMin = 20f;
    #region Tooltip
    [Tooltip("The maximum speed of the ammo - the speed will be a random value between the min and max")]
    #endregion
    public float ammoSpeedMax = 20f;
    #region Tooltip
    [Tooltip("The range of the ammo/ammo pattern in unity units")]
    #endregion
    public float ammoRange = 20f;
    #region Tooltip
    [Tooltip("The rotation speed of the ammo pattern (degrees per second)")]
    #endregion
    public float ammoRotationSpeed = 1f;

    #region Header AMMO SPREAD DETAILS
    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("This is the minimum spread angle of the ammo. A higher spread means less accuracy. A random spread is calculated between the min and max values")]
    #endregion
    public float ammoSpreadMin = 0f;
    #region Tooltip
    [Tooltip("This is the maximum spread angle of the ammo. A higher spread means less accuracy. A random spread is calculated between the min and max values")]
    #endregion
    public float ammoSpreadMax = 0f;

    #region Header AMMO SPAWN DETAILS
    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("This is the minimum amount of ammo spawned per shot. A random number is spawned between the min and max values")]
    #endregion
    public int ammoSpawnAmountMin = 0;
    #region Tooltip
    [Tooltip("This is the maximum amount of ammo spawned per shot. A random number is spawned between the min and max values")]
    #endregion
    public int ammoSpawnAmountMax = 0;
    #region Tooltip
    [Tooltip("Minimum spawn interval time (in seconds) between spawned ammo. A random value is used between the min and max values")]
    #endregion
    public float ammoSpawnIntervalMin = 0f;
    #region Tooltip
    [Tooltip("Maximum spawn interval time (in seconds) between spawned ammo. A random value is used between the min and max values")]
    #endregion
    public float ammoSpawnIntervalMax = 0f;

    #region Header AMMO TRAIL DETAILS
    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Selected if ammo trail is required, otherwise deselect. If selected, then the rest of the ammo trail values should be populated")]
    #endregion
    public bool isAmmoTrail = false;
    #region Tooltip
    [Tooltip("Ammo trail lifetime in seconds")]
    #endregion
    public float ammoTrailTime = 3f;
    #region Tooltip
    [Tooltip("Ammo trail material")]
    #endregion
    public Material ammoTrailMaterial;
    #region Tooltip
    [Tooltip("Ammo trail start width")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    #region Tooltip
    [Tooltip("Ammo trail end width")]
    #endregion
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region VALIDTION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0f)
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        }
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
#endif
        #endregion
    }
}
