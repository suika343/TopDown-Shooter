using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField] private float lightIntensityMin;
    [SerializeField] private float lightIntensityMax;
    [SerializeField] private float lightFlicketTimeMin;
    [SerializeField] private float lightFlicketTimeMax;
    private float lightFlickerTimer;

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        lightFlickerTimer = Random.Range(lightFlicketTimeMin, lightFlicketTimeMax);
    }

    private void Update()
    {
        if(light2D == null) return;

        lightFlickerTimer -= Time.deltaTime;

        if(lightFlickerTimer <= 0f)
        {
            lightFlickerTimer = Random.Range(lightFlicketTimeMin, lightFlicketTimeMax);

            Randomizer2DLightIntensity();
        }
    }

    private void Randomizer2DLightIntensity()
    {
        light2D.intensity = Random.Range(lightIntensityMin, lightIntensityMax);
    }

    #region VALIDATION

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightIntensityMin), lightIntensityMin, nameof(lightIntensityMax), lightIntensityMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(lightFlicketTimeMin), lightFlicketTimeMin, nameof(lightFlicketTimeMax), lightFlicketTimeMax, false);
    }
#endif
    #endregion
}
