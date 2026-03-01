using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with child TrailRenderer Component")]
    #endregion
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; //the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement; //for ammo patterns

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //ammo charge effect
        if(ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        //Calculate distance vector to move ammo
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        //disable ammo if it has reached max range
        ammoRange -= distanceVector.magnitude;
        if (ammoRange <= 0)
        {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShowAmmoHitEffect();
        DisableAmmo();

    }

    /// <summary>
    /// Initialize the ammo being fired
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="ammoSpeed"></param>
    /// <param name="weaponAimDirectionVector"></param>
    /// <param name="overrideAmmoMovement"></param>
    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, 
        float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region AMMO
        this.ammoDetails = ammoDetails;

        //Set Fire Direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        //Set Ammo Sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        //Set Initial Ammo Material
        if(ammoDetails.ammoChargeTime > 0f)
        {
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            isAmmoMaterialSet = false;
        }
        else
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            ammoChargeTimer = 0f;
            isAmmoMaterialSet = true;
        }

        //Set ammo Range
        ammoRange = ammoDetails.ammoRange;

        //Set ammo speed
        this.ammoSpeed = ammoSpeed;

        //Override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        //Actiave ammo gameObject (object pool)
        gameObject.SetActive(true);
        #endregion

        #region AMMO TRAILS
        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false); 
        }
        #endregion
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the random spread
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        //calculate random spread
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        //randomly decide to add or subtract spread
        int spreadDirection = Random.Range(0, 2) * 2 - 1;

        //check if distance between player and weapon aim is less than threshold to use aim angle
        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        //apply spread
        fireDirectionAngle += spreadDirection * randomSpread;

        //set ammo rotation
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        //set ammo fire direction
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    private void ShowAmmoHitEffect()
    {
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            AmmoHitEffect ammoHitEffect = PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, 
                transform.position, Quaternion.identity) as AmmoHitEffect;

            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);

            ammoHitEffect.gameObject.SetActive(true);
        }

    }
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}
