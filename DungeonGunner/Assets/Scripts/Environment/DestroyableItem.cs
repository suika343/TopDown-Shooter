using UnityEngine;
using System.Collections;
using System;

[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] private int startingHealthAmount = 1;
    [Header("SOUND EFFECT")]
    [SerializeField] private SoundEffectSO destroySoundEffect;
    
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        receiveContactDamage = GetComponent<ReceiveContactDamage>();

        health.SetStartingHealth(startingHealthAmount);
    }


    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0f)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        //destory collider component
        Destroy(boxCollider2D);

        //play sfx
        if(destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        //trigger animation
        animator.SetBool(Settings.destroy, true);

        while(!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        //Destroy all components except sprite renderer
        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);
    }
}
