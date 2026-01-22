using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    #region
    [Tooltip("Populate with the SpriteRenderer on the child Weapon GameObject")]
    #endregion
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    #region
    [Tooltip("Populate with the PolygonCollider2D on the child Weapon GameObject")]
    #endregion
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    #region
    [Tooltip("Populate with the Transform of the WeaponShootPosition GameObject")]
    #endregion
    [SerializeField] private Transform weaponShootPositionTransform;
    #region
    [Tooltip("Populate with the Transform of the WeaponEffectPosition GameObject")]
    #endregion
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        //Set weapon sprite
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        //If the weapon has a polygon collider, then set it to the weapon sprite physics shape
        if(weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            //Get sprite physics shape 
            List<Vector2> spritePhysicsShapePointList = new List<Vector2>();
            //this returns the sprite physics shape points
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointList);

            //set polygon collider 2d points to sprite physics shape points
            weaponPolygonCollider2D.points = spritePhysicsShapePointList.ToArray();
        }

        //Set weapon shoot position
        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetAmmoDetails()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetWeaponShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetWeaponShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion
}
