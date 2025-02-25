using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
    

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] 
    public string id;
    [HideInInspector] 
    public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] 
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

        // Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)
        int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        roomNodeType = roomNodeTypeList.list[selection];
        this.name = "RoomNode" + "_" + roomNodeType.roomNodeTypeName;
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
        childRoomNodeIDList.Add(childId);
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

#endif
    #endregion
}
