using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform weaponShootPosition;
    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;


    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;

        firingIntervalTimer = InitializeFiringIntervalTimer();
        firingDurationTimer = InitializeFiringDurationTimer();
    }

    private void Update()
    {
        firingIntervalTimer -= Time.deltaTime;

        if(firingIntervalTimer <= 0)
        {
            if(firingDurationTimer > 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                //reset timers
                firingIntervalTimer = InitializeFiringIntervalTimer();
                firingDurationTimer = InitializeFiringDurationTimer();
            }
        }
    }

    private void FireWeapon()
    {
        //PlayerDistance Direction from enemy pivot point to player
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        //Direction Vector of player from weapon shoot position
        Vector3 weaponDirection = (GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position);

        //Get weapon to player angle in degrees
        float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        //Get enemy to player angle in degrees
        float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

        AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

        //Trigger weapon aim event
        enemy.aimWeaponEvent.CallWeaponAimEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);

        //only fire if enemy has a weapon
        if(enemyDetails.enemyWeapon != null)
        {
            //get range of ammo
            float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;

            //only fire if the enemy is in range of the ammo
            if(playerDirectionVector.magnitude <= enemyAmmoRange)
            {
                //check if line of sight is require before firing
                if(enemyDetails.isFiringLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange))
                {
                    return;
                }

                //fire weapon event
                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees, weaponDirection);
            }
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 direction, float range)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)direction, range, layerMask);

        if(raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }

    private float InitializeFiringIntervalTimer()
    {
        return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
    }

    private float InitializeFiringDurationTimer()
    {
        return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
    }
}
