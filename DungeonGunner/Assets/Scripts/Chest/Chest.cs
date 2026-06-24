using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUsable
{
    #region Tooltip
    [Tooltip("Set this to the color to be used for the materialization effect")]
    #endregion
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;
    #region Tooltip
    [Tooltip("Set this to the time it takes for the materialization effect to complete")]
    #endregion
    [SerializeField] private float materializeTime = 1f;
    #region Tooltip
    [Tooltip("Populate with ItemSpawnPoint Transform")]
    #endregion
    [SerializeField] private Transform itemSpawnPoint;

    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;

    private bool isEnabled = false;

    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextTMP = GetComponent<TextMeshPro>();
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetailsSO, int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetailsSO;
        this.ammoPercent = ammoPercent;

        if(shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeCoroutine(GameResources.Instance.materializeShader, 
            materializeColor, materializeTime,
            spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    private void EnableChest()
    {
       isEnabled = true;
    }

    public void Use()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            case ChestState.empty:
                return;
            default:
                return;
        }
    }

    private void OpenChest()
    {
        //play animation
        animator.SetBool(Settings.use, true);
        
        //play sfx
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);

        //check if player already has the weapon
        if(weaponDetails != null)
        {
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
            {
                weaponDetails = null;
            }
        }

        UpdateChestState();
    }

    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            InstantiateAmmoItem();
        }
        else if (weaponDetails != null)
        {
            chestState = ChestState.weaponItem;
            InstantiateWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIconSprite, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem();

        chestItem.Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor);
    }

    private void InstantiateAmmoItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletIconSprite, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }

    private void CollectHealthItem()
    {
        //check if item exists and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //Add Health to Player
        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

        //Play pickup SFX
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup);

        healthPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private void CollectWeaponItem()
    {
        //check if item exists and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        //If the player doesnt have the weapon yet, add the weapon
        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
        {
            //Add weapon to player
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

            //Play pickup SFX
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
        }
        else
        {
            //display message saying you already have the weapon
            StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f));
        }

        weaponDetails = null;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }
    
    private void CollectAmmoItem()
    {
        //check if item exists and has been materialized
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();

        //Update Ammo for Current Weapon
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

        //Play pickup SFX
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);

        ammoPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextTMP.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        messageTextTMP.text = "";
    }
}
