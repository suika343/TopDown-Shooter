using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;
using Unity.VisualScripting;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthEvent))]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Enemy enemy;

    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //trigger health event at the start for UI Update
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        //get player / enemy hit immunity details
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
            isRolling = player.playerControl.isPlayerRolling;

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();
        }

        /*
        if(isDamageable && isRolling)
        {
            Debug.Log("---Player dodged damage by rolling");
        }

        if (!isDamageable && isRolling)
        {
            Debug.Log("---Player avoided damage due to post hit immunity");
        }*/
    }

    private void PostHitImmunity()
    {
        if (!gameObject.activeSelf)
            return;

        if (isImmuneAfterHit)
        {
            if(immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        //amount of times the player is tinted red then back to normal (hence the /2f)
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return WaitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        isDamageable = true;
    }

    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthChangedEvent((float)currentHealth / startingHealth, currentHealth, damageAmount);
    }

    /// <summary>
    /// Set starting health
    /// </summary>
    /// <param name="startingHealth"></param>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// Get starting health
    /// </summary>
    /// <returns></returns>
    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
