using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    
    #region HEADER BASIC LEVEL DETAILS
    [Space(10)]
    [Header("Basic Level Details")]
    #endregion

    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip

    public string levelName;

    #region HEADER ROOM TEMPLATE FOR LEVEL
    [Space(10)]
    [Header("ROOM TEMPLATE FOR LEVEL")]
    #endregion
  
    #region Tooltip
    [Tooltip("Populate the list with the room templates that  you want to be part of the level. You need to ensure that room templates are included " +
        "for all rooms node types that are specified in the Room Node Graphs for the level.")]
    #endregion Tooltip

    public List<RoomTemplateSO> roomTemplatesList;

    #region HEADER ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion

    #region Tooltip
    [Tooltip("Populate this list with the room node graphs that will be randomly selected for the leve.")]
    #endregion Tooltip

    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        if(HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplatesList), roomTemplatesList))
        {
            return;
        }

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        //Check to make sure that room templates are included for all room node types in the room node graphs
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        //loop through room templates and check for room node types
        foreach (RoomTemplateSO roomTemplate in roomTemplatesList)
        {
            if(roomTemplate == null)
            {
                return;
            }

            if (roomTemplate.roomNodeType.isCorridorEW)
            {
                isEWCorridor = true;
            }

            if (roomTemplate.roomNodeType.isCorridorNS)
            {
                isNSCorridor = true;
            }

            if (roomTemplate.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }

        if (!isEWCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : no E/W Corridor Room Type specified");
        }

        if(!isNSCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : no N/S Corridor Room Type specified");
        }

        if(!isEntrance)
        {
            Debug.Log("In " + this.name.ToString() + " : no Entrance Room Type specified");
        }

        //Loop through all room node graphs and check that there is a room template for each room node type
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if(roomNodeGraph == null)
            {
                return;
            }

            //loop through room nodes in the room node graph
            foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
            {
                if(roomNode == null)
                {
                    continue;
                }

                //Check if a room template is specified for each Room Node Type

                //Corridors and Entances already checked
                if (roomNode.roomNodeType.isEntrance || roomNode.roomNodeType.isCorridorEW || roomNode.roomNodeType.isCorridorNS 
                    || roomNode.roomNodeType.isCorridor || roomNode.roomNodeType.isNone)
                {
                    continue;
                }

                bool isRoomNodeTypeFound = false;

                //Loop though room templates and check for room node type
                foreach (RoomTemplateSO roomTemplate in roomTemplatesList)
                {
                    if (roomTemplate == null)
                    {
                        continue;
                    }
                    if (roomTemplate.roomNodeType == roomNode.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + " : no Room Template: " + roomNode.roomNodeType.name.ToString()
                        + " specified for Room Node Graph " 
                        + " in " + roomNodeGraph.name.ToString());
                }
            }
        }

    }

#endif
    #endregion
}
