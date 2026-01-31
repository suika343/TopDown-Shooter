using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodetypeList;


    #region PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    #region Tooltip
    [Tooltip("The current player ScriptableObject - this is used to reference the player between the scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region MATERIALS
    [Space(10)]
    [Header("Materials")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;
    #region Tooltip
    [Tooltip("Sprite-Lit Default Material")]
    #endregion
    public Material litMaterial;
    #region Tooltip
    [Tooltip("Populate with Variable Lit Shader")]
    #endregion
    public Shader variableLitShader;

    #region HEADER UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with Ammo Icon prefab")]
    #endregion
    public GameObject ammoIconPrefab;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodetypeList), roomNodetypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }
#endif
#endregion
}