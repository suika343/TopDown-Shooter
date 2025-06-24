using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region H#ADER
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion HEADER

    #region Tooltip
    [Tooltip("The minimum movement speed. The GetMoveSpeed method calculates a random value between the minimum and the maximum")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;

    #region Tooltip
    [Tooltip("The maximum movement speed. The GetMoveSpeed method calculates a random value between the minimum and the maximum")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;

    #region Tooltip
    [Tooltip("Roll speed - if there is roll movement")]
    #endregion Tooltip
    public float rollSpeed; //for player

    #region Tooltip
    [Tooltip("Roll distance - if there is roll movement")]
    #endregion Tooltip
    public float rollDistance; //for player

    #region Tooltip
    [Tooltip("Roll cooldown - if there is roll movement")]
    #endregion Tooltip
    public float rollCooldownTime; //for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values.
    /// </summary>
    /// <returns></returns>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return maxMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
        if(rollSpeed != 0f || rollDistance != 0f || rollCooldownTime != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
    #endregion
}
