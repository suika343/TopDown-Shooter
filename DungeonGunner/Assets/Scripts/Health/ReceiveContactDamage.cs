using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
    #region HEADER
    [Header("The Contact Damage amount to receive")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount = 0)
    {
        if(contactDamageAmount > 0)
        {
            damageAmount = contactDamageAmount;
        }

        //Debug.Log("Potential damage amount of " + damageAmount);

        health.TakeDamage(damageAmount);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
}
