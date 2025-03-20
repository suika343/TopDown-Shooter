using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector]
    public string guid;

    #region Header ROOM PREFAB
    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion ROOM PREFAB
    #region Tooltip
    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]
    #endregion Tooltip

    public GameObject prefab;
    [HideInInspector]
    public GameObject previousPrefab; // this is used to regenerate the guid if the SO is copied and the prefab is changed

    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion Header ROOM CONFIGURATION

    #region Tooltip
    [Tooltip("The Room Node Type Scriptable Object. The room node types correspond to the room nodes used in the room node graph." +
        "The exceptions being with corridors. In the Room Node Graph there is just one corridor type 'Corridor'" +
        "For the Room Templates, there are 2 corridor node types - CorridorNS and CorridorEW")]
    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner " +
        "of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position " +
        "for that bottom left corner Note: this is the local tilemap position and NOT the world position.")]
    #endregion Tooltip
    public Vector2Int lowerBounds;

    #region Tooltip
    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room uper bounds represent the top right corner " +
        "of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position " +
        "for that bottom left corner Note: this is the local tilemap position and NOT the world position.")]
    #endregion Tooltip
    public Vector2Int upperBounds;

    #region Tooltip
    [Tooltip("There should be a maximum of 4 doorways in a room - one for each compass direction. These should have a consistent 3 tile opening size, " +
        "with the middle tile position being the doorway coordinate 'position'.")]
    #endregion Tooltip
    [SerializeField]
    public List<Doorway> doorwayList;

    #region Tooltip
    [Tooltip("Each possible spawwn position (used for enemies and chests) for the room in the tilemap coordinates should be added in this array")]
    #endregion Tooltip

    public Vector2Int[] spawnPositionsArray;

    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }
    #region EDITOR VALIDATION
#if UNITY_EDITOR
    //set unique GUID if the SO is duplicated but the prefab is different or if it is empty
    private void OnValidate()
    {
        if(guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionsArray), spawnPositionsArray);
    }
#endif
    #endregion EDITOR VALIDATION
}
