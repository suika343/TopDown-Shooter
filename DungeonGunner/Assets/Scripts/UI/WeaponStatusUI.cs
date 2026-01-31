using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WeaponStatusUI : MonoBehaviour
{
    #region HEADER UI ELEMENTS
    [Space(10)]
    [Header("UI Elements")]
    #endregion
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBarTransform;
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcon(weapon);

        //if weapon is stil reloading, update the reload bar
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar(weapon);
        }

        UpdateReloadText(weapon);
    }

    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcon(weapon);
        UpdateReloadText(weapon);
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarCoroutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarCoroutine(Weapon weapon)
    {
        barImage.color = Color.red;

        while(weapon.isWeaponReloading)
        {
            float reloadProgress = weapon.weaponReloadTimer / weapon.weaponDetails.weaponReloadTime;
            reloadBarTransform.localScale = new Vector3(reloadProgress, 1f, 1f);
            yield return null;
        }
    }
    private void WeaponReloaded(Weapon weapon)
    {
        //if weapon reloaded is current weapon
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcon(weapon);
            ResetWeaponReloadBar(weapon);
        }
    }

    private void ResetWeaponReloadBar(Weapon weapon)
    {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;
        reloadBarTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ") " + weapon.weaponDetails.weaponName.ToUpper();
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponTotalRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    private void UpdateAmmoLoadedIcon(Weapon weapon)
    {
        //clear existing icons
        ClearAmmoLoadedIcons(weapon);
        //create new icons
        for (int i = 0; i < weapon.weaponClipAmmoRemaining; i++)
        {
            //instantiate ammo icon prefabs
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }

    }

    private void ClearAmmoLoadedIcons(Weapon weapon)
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if (!weapon.weaponDetails.hasInfiniteClipCapacity && (weapon.weaponClipAmmoRemaining <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;
            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextCoroutine());
        }
        else
        {
            reloadText.text = "";
            StopBlinkingReloadText();
        }
    }

    private IEnumerator StartBlinkingReloadTextCoroutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }


    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBarTransform), reloadBarTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion
}
