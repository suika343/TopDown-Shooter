using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    #region HEADER
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    public string enemyName;

    public GameObject enemyPrefab;

    public float chaseDistance = 50f;

    #region HEADER
    [Space(10)]
    [Header("ENEMY MATERIAL")]
    #endregion
    public Material enemyStandardMaterial;

    #region HEADER
    [Space(10)]
    [Header("ENEMY MATERIALIZE EFFECT SETTINGS")]
    #endregion
    public float enemyMaterializeEffectTime;
    public Shader enemyMaterializeShader;
    [ColorUsageAttribute(false, true)] public Color enemyMaterializeColor;

    #region HEADER
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion
    public WeaponDetailsSO enemyWeapon;
    public float firingIntervalMin = 0.1f;
    public float firingIntervalMax = 1f;
    public float firingDurationMin = 1f;
    public float firingDurationMax = 2f;
    public bool isFiringLineOfSightRequired;

    #region HEADER ENEMY HEALTH
    [Space(10)]
    [Header("ENEMY HEALTH")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;
    public bool isImmuneAfterHit = false;
    public float hitImmunityTime;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeEffectTime), enemyMaterializeEffectTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif
    #endregion
}
