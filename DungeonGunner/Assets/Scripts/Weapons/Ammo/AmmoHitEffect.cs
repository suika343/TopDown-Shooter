using UnityEngine;

public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO shootEffect)
    {
        //color gradient
        SetShootEffectColorGradient(shootEffect.colorGradient);

        //particle starting values
        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticleSize, shootEffect.startPartcileSpeed,
            shootEffect.startParticleLifetime, shootEffect.effectGravity, shootEffect.maxParticles);

        //emmission values, burst particle values
        SetShootEffectParticleEmmission(shootEffect.emmisionRate, shootEffect.burstParticleCount);

        //particle sprite
        SetParticleSprite(shootEffect.sprite);

        //Velocity over lifetime
        SetShootEffectVelocityOverLifetime(shootEffect.veclocityOverLifetimeMin, shootEffect.veclocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startPartcileSpeed,
        float startParticleLifetime, float effectGravity, int maxParticles)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startPartcileSpeed;
        mainModule.startLifetime = startParticleLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticles;
    }

    private void SetShootEffectParticleEmmission(float emmisionRate, int burstParticleCount)
    {
        ParticleSystem.EmissionModule emmissionModule = ammoHitEffectParticleSystem.emission;
        emmissionModule.rateOverTime = emmisionRate;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, (short)burstParticleCount);
        emmissionModule.SetBurst(0, burst);
    }

    private void SetParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.mode = ParticleSystemAnimationMode.Sprites;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetShootEffectVelocityOverLifetime(Vector3 veclocityOverLifetimeMin, Vector3 veclocityOverLifetimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        //X
        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = veclocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = veclocityOverLifetimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        //Y
        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = veclocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = veclocityOverLifetimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        //Z
        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = veclocityOverLifetimeMin.z;
        minMaxCurveZ.constantMax = veclocityOverLifetimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
