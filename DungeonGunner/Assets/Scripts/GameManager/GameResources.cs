using UnityEngine;
using System.Collections.Generic;
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

    #region HEADER MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicOnLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;

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
    #region Tooltip
    [Tooltip("Populate with Materialize Shader")]
    #endregion
    public Shader materializeShader;

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

    #region HEADER CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with Chest Item prefab")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("Populate with Chest Icon Sprite")]
    #endregion
    #region Tooltip
    public Sprite chestIconSprite;
    [Tooltip("Populate with Heart Icon Sprite")]
    #endregion
    public Sprite heartIconSprite;
    #region Tooltip
    [Tooltip("Populate with Bullet Icon Sprite")]
    #endregion
    public Sprite bulletIconSprite;

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
    #region Tooltip
    [Tooltip("Populate with table flipping sound effect")]
    #endregion
    public SoundEffectSO tableFlipSoundEffect;
    #region Tooltip
    [Tooltip("Populate with chest open sound effect")]
    #endregion
    public SoundEffectSO chestOpen;
    #region Tooltip
    [Tooltip("Populate with health pickup sound effect")]
    #endregion
    public SoundEffectSO healthPickup;
    #region Tooltip
    [Tooltip("Populate with weapon pickup sound effect")]
    #endregion
    public SoundEffectSO weaponPickup;
    #region Tooltip
    [Tooltip("Populate with ammo pickup sound effect")]
    #endregion
    public SoundEffectSO ammoPickup;

    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion Header MINIMAP
    public GameObject minimapSkullPrefab;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodetypeList), roomNodetypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickup), ammoPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickup), weaponPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickup), healthPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestIconSprite), chestIconSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnLowSnapshot), musicOnLowSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartIconSprite), heartIconSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIconSprite), bulletIconSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlipSoundEffect), tableFlipSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMixerGroup), soundsMixerGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
    }
#endif
#endregion
}