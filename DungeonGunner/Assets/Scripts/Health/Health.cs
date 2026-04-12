using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthEvent))]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;

    [HideInInspector] public bool isDamagable = true;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //trigger health event at the start for UI Update
        CallHealthEvent(0);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isDamagable) return;
        currentHealth -= damageAmount;
        CallHealthEvent(damageAmount);
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
