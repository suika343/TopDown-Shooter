using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    #region HEADER Deal Damage
    [Header("Deal Damage")]
    #endregion
    #region TOOLTIP
    [Tooltip("The contact damage to deal (overridden by the receiver)")]
    #endregion
    [SerializeField] private int contactDamageAmount;
    [Tooltip("Specify which layers can receive contact damage")]
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding)
            return;

        ContactDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isColliding)
            return;

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        //check if collision object is in the correct layer
        //using bitwise operation
        int collisionLayerMask = 1 << collision.gameObject.layer;

        if ((layerMask.value & collisionLayerMask) == 0)
            return;

        //check if the collision object has a ReceiveContactDamage component
        ReceiveContactDamage receiveContactDamage = collision.GetComponent<ReceiveContactDamage>();

        if (receiveContactDamage != null) 
        { 
            isColliding = true;
            //Reset contact damage timer after set time
            Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);
            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    private void ResetContactCollision()
    {
        isColliding = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif

}
