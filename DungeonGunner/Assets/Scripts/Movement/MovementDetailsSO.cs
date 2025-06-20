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
    }
#endif
    #endregion
}
