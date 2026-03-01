using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    #region HEADER
    [Space(10)]
    [Header("AMMO HIT EFFECT DETAILS")]
    #endregion
    public Gradient colorGradient;

    public float duration = 0.50f;
    public float startParticleSize = 0.25f;
    public float startPartcileSpeed = 3f;
    public float startParticleLifetime = 0.5f;

    public int maxParticles = 100;
    public int emmisionRate = 100;
    public int burstParticleCount = 20;

    public float effectGravity = -0.01f;
    public Sprite sprite;

    public Vector3 veclocityOverLifetimeMin;
    public Vector3 veclocityOverLifetimeMax;

    public GameObject ammoHitEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startPartcileSpeed), startPartcileSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleLifetime), startParticleLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticles), maxParticles, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emmisionRate), emmisionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleCount), burstParticleCount, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
    }
#endif
    #endregion
}
