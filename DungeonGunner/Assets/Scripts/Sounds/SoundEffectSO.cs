using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sound/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    #region HEADER Sound Effect Details
    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The name of the sound effect")]
    #endregion
    public string soundEffectName;
    #region Tooltip
    [Tooltip("The prefab for the sound effect")]
    #endregion
    public GameObject soundPrefab;
    #region Tooltip
    [Tooltip("The audio clip for the sound effect")]
    #endregion
    public AudioClip soundEffectClip;
    #region Tooltip
    [Tooltip("The minumum pitch variation for the sound effect. A random pitch variation will be generated between the min" +
        "and the max values.")]
    #endregion
    public float soundEffectRandomPitchVariationMin = 0.8f;
    #region Tooltip
    [Tooltip("The maximum pitch variation for the sound effect. A random pitch variation will be generated between the min" +
        "and the max values.")]
    #endregion
    public float soundEffectRandomPitchVariationMax = 1.2f;
    #region Tooltip
    [Tooltip("The volume for the sound effect")]
    #endregion
    public float soundEffectVolume;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectRandomPitchVariationMin), soundEffectRandomPitchVariationMin, 
            nameof(soundEffectRandomPitchVariationMax), soundEffectRandomPitchVariationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
