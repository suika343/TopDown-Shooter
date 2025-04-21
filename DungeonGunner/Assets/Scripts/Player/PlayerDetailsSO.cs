using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The name of the player character.")]
    #endregion
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("The gameObject for the player.")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("Player Runtime Animator Controller.")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("HEALTH")]
    #endregion
    #region Tooltip
    [Tooltip("Player starting health amount.")]
    #endregion
    public int playerHealthAmount;

    #region Header OTHER
    [Space(10)]
    [Header("OTHER")]
    #endregion
    #region Tooltip
    [Tooltip("Player icon sprite to be used on the minimap.")]
    #endregion
    public Sprite playerMinimapIcon;
    #region Tooltip
    [Tooltip("Player hand sprite.")]
    #endregion
    public Sprite playerHandSprite;
    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Check for empty strings
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        // Check for null values
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        //Check for non-zero positive values
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
    }
#endif
    #endregion
}

