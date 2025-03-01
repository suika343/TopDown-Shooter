using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
    

public class RoomNodeSO : ScriptableObject
{
    //[HideInInspector] 
    public string id;
    //[HideInInspector] 
    public List<string> parentRoomNodeIDList = new List<string>();
    //[HideInInspector] 
    public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    #region EDITOR CODE
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    /// <summary>
    /// Initialize
    /// </summary>
    public void Initialize(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;
    
        // Load room node type list
        roomNodeTypeList = GameResources.Instance.roomNodetypeList;
    }

    /// <summary>
    /// Draw node with the node style
    /// </summary>
    public void Draw(GUIStyle nodeStyle)
    {
        //Draw node box during begin area
        GUILayout.BeginArea(rect, nodeStyle);

        //Start Region to Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label that cannnot be edited
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];

            //if the room type selection has changed making child connections potentially invalid
            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor ||
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor ||
                roomNodeTypeList.list[selected].isBossRoom && !roomNodeTypeList.list[selection].isBossRoom 
                )
            {
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        //Get Child Room Node
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        if (childRoomNode != null)
                        {
                            //remove child id from the room node list
                            RemoveChildRoomNodeIdFromRoomNode(childRoomNode.id);

                            //remove parent id from the child room node
                            childRoomNode.RemoveParentoomNodeIdFromRoomNode(id);
                        }
                    }
                }        
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(this);
        }
        
        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray =  new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayNodeGraphInEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            //Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            //Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    public void ProcessMouseDownEvent(Event currentEvent)
    {
        //Left click down
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        //Right Click Down
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    public void ProcessMouseUpEvent(Event currentEvent)
    {
        //Left click Up
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
        else if (currentEvent.button == 1)
        {
            ProcessLeftClickUpEvent();
        }
    }
    public void ProcessMouseDragEvent(Event currentEvent)
    {
        //process left click drag
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    public void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        //Toggle node selection
        isSelected = !isSelected;
    }

    public void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    public void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawLineConnectionFrom(this, currentEvent.mousePosition);
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Add Room Node Id to Child Room Node List (returns true if iti s added, returns false otherwise)
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public bool AddChildRoomNodeIdToRoomNode(string childId)
    {
        if (IsChildRoomValid(childId))
        {
            childRoomNodeIDList.Add(childId);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the child room node can be validly added to the parent node return true if valid, false otherwise
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public bool IsChildRoomValid(string childId)
    {
        bool isConnectedBossRoomAlready = false;
        //Check if there is already a connected boss room in the node graph
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossRoomAlready = true;
            }
        }
        
        //if child node is a boss room and there is already a boss room connected in the graph - return false
        if (roomNodeGraph.GetRoomNode(childId).roomNodeType.isBossRoom && isConnectedBossRoomAlready)
        {
            return false;
        }

        //if the child node is none - return false
        if (roomNodeGraph.GetRoomNode(childId).roomNodeType.isNone)
        {
            return false;
        }

        //if the node already is a child of this room node - return false
        if (childRoomNodeIDList.Contains(childId))
        {
            return false;
        }

        //cannot connect a room node to itself - return false
        if (childId == id)
        {
            return false;
        }

        //if the node is a parent of this node - return false
        if (parentRoomNodeIDList.Contains(childId))
        {
            return false;
        }

        //if the node already has a parent - return false
        if (roomNodeGraph.GetRoomNode(childId).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }

        //cannot connect corridors to each other
        if (roomNodeGraph.GetRoomNode(childId).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }

        //cannot connect rooms to rooms (without corridors)
        if (!roomNodeGraph.GetRoomNode(childId).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }

        //if adding a corridor and this node already has the max number of child corridors
        if (roomNodeGraph.GetRoomNode(childId).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }

        //the entrance node must be the top level parent node
        if (roomNodeGraph.GetRoomNode(childId).roomNodeType.isEntrance)
        {
            return false;
        }

        //if adding a room to a corridor, check that this corridor doesnt already have a room added to it
        if (!roomNodeGraph.GetRoomNode(childId).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Add Room Node Id to Parent Room Node List (returns true if iti s added, returns false otherwise)
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public bool AddParentRoomNodeIdToRoomNode(string parentId)
    {
        parentRoomNodeIDList.Add(parentId);
        return true;
    }
    
    /// <summary>
    /// Remove childID from the child room node list (returns true if noide has beed removed, false otherwise)
    /// </summary>
    /// <param name="childId"></param>
    /// <returns></returns>
    public bool RemoveChildRoomNodeIdFromRoomNode(string childId)
    {
        //if the node contains the child id then remove it
        if (childRoomNodeIDList.Contains(childId))
        {
            childRoomNodeIDList.Remove(childId);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove parentId from the parent room node list (returns true if noide has beed removed, false otherwise)
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public bool RemoveParentoomNodeIdFromRoomNode(string parentId)
    {
        //if the node contains the child id then remove it
        if (parentRoomNodeIDList.Contains(parentId))
        {
            parentRoomNodeIDList.Remove(parentId);
            return true;
        }
        return false;
    }

#endif
    #endregion
}
