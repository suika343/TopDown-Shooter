using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

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

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    #region Tooltip
    [Tooltip("Collision tiles that the enemies can't walk through (purple tiles)")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("Preferred path tiles for enemy navigation (green tiles)")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region HEADER UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with Ammo Icon prefab")]
    #endregion
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("Populate with Heart prefab")]
    #endregion
    public GameObject heartPrefab;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion Header
    #region Tooltip
    [Tooltip("Populate with the sounds mixer group")]
    #endregion
    public AudioMixerGroup soundsMixerGroup;
    #region Tooltip
    [Tooltip("Populate with door opening sound effect")]
    #endregion
    public SoundEffectSO doorOpenCloseSoundEffect;

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMixerGroup), soundsMixerGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
    }
#endif
#endregion
}