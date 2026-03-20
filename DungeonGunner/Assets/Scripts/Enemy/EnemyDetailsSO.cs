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
    }
#endif
    #endregion
}
