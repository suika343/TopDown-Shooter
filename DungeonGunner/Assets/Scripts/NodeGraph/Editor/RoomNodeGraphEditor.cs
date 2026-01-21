using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle selectedRoomNodeStyle;
    private GUIStyle chestRoomNodeStyle;
    private GUIStyle entraceRoomNodeStyle;
    private GUIStyle bossRoomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    //Node Layout Values
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    //Connecting line width
    private float connectingLineWidth = 3f;
    private float connectingArrowSize = 6f;

    //grid values
    private const float gridSizeLarge = 100f;
    private const float gridSizeSmall = 20f;

    private int smallRoomsCount = 0;
    private int mediumRoomsCount = 0;
    private int largeRoomsCount = 0;
    private int chestRoomsCount = 0;
    private int totalRoomsCount = 0;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    public static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        //Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;
        
        //Define node layout style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        entraceRoomNodeStyle = new GUIStyle();
        entraceRoomNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
        entraceRoomNodeStyle.normal.textColor = Color.white;
        entraceRoomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        entraceRoomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        chestRoomNodeStyle = new GUIStyle();
        chestRoomNodeStyle.normal.background = EditorGUIUtility.Load("node5") as Texture2D;
        chestRoomNodeStyle.normal.textColor = Color.white;
        chestRoomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        chestRoomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);


        bossRoomNodeStyle = new GUIStyle();
        bossRoomNodeStyle.normal.background = EditorGUIUtility.Load("node6") as Texture2D;
        bossRoomNodeStyle.normal.textColor = Color.white;
        bossRoomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        bossRoomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //Define selected layout style
        selectedRoomNodeStyle = new GUIStyle();
        selectedRoomNodeStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        selectedRoomNodeStyle.normal.textColor = Color.white;
        selectedRoomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        selectedRoomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // Load Room Node Types
        roomNodeTypeList = GameResources.Instance.roomNodetypeList;
    }

    private void OnDisable()
    {
        //unsubscribe from inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.EntityIdToObject(instanceID) as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        // If a scriptable object of type RoomNodeGraphSO has been selected then process
        if (currentRoomNodeGraph != null)
        {
           
            //Draw Background grid lines
            DrawGridLines(gridSizeSmall, .2f, Color.gray);
            DrawGridLines(gridSizeLarge, .3f, Color.gray);
            //Draw Dragged Lines - Connecting line between room nodes
            DrawDraggedLine();

            //Process Events
            ProcessEvents(Event.current);

            //Draw Room Node Connections
            DrawRoomNodeConnections();

            //Draw Room Nodes
            DrawRoomNodes();

            CountRoomTypes();
            GUI.Label(new Rect(5, 5, 200, 100), "Room Nodes: " + currentRoomNodeGraph.roomNodeList.Count);
            GUI.Label(new Rect(5, 25, 200, 100), "Small Rooms: " + smallRoomsCount);
            GUI.Label(new Rect(5, 45, 200, 100), "Medium Rooms: " + mediumRoomsCount);
            GUI.Label(new Rect(5, 65, 200, 100), "Large Rooms: " + largeRoomsCount);
            GUI.Label(new Rect(5, 85, 200, 100), "Chest Rooms: " + chestRoomsCount);
            GUI.Label(new Rect(5, 105, 200, 100), "Total Rooms: " + totalRoomsCount);
        }


        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawDraggedLine()
    {
        if(currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent) {

        //Reset Graph Drag
        graphDrag = Vector2.zero;

        // Get room node that mouse is over if it is null or if it is not currently dragging one
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        //if mosue is not over a room node
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);

        }
        // else process room node events
        else
        {
            //process room node events
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// Process ROom Node Graph Events
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvents(currentEvent); 
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvents(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvents(currentEvent);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Process Mouse Down Events on the Room Node Graph (not over a node)
    /// </summary>
    private void ProcessMouseDownEvents(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if(currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    /// <summary>
    /// Process Mouse Drag Events on the Room Node Graph
    /// </summary>
    private void ProcessMouseDragEvents(Event currentEvent)
    {
        //Right Mouse Button
        if(currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if(currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for(int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(graphDrag);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Process Mouse Up Events on the Room Node Graph
    /// </summary>
    private void ProcessMouseUpEvents(Event currentEvent)
    {
        // if releasing right mouse button while dragging a line
        if(currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            //Check if Mouse is over roomNode
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);


            if(roomNode != null)
            {
                //set it as child of the parent room node if it can be added

                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIdToRoomNode(roomNode.id))
                {
                    // set parent id in child room node

                    roomNode.AddParentRoomNodeIdToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Show Context Menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a room node at the Mouse Position
    /// </summary>
    public void CreateRoomNode(object mousePositionObject)
    {

        //if current room node graph is empty, create entrance room node
        if(currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200, 200), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// Create a room node type at the Mouse Position - overloaded to also pass Room Node Type
    /// </summary>
    public void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create room node scriptable object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // add room node to current room node graph room node list
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // set room node values
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // add room node to room node graph scriptable object asset database

        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets(); 

        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// Deletes the link between the selected room nodes
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    //Get Child Room Node
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if(childRoomNode != null)
                    {
                        //remove child id from the room node list
                        roomNode.RemoveChildRoomNodeIdFromRoomNode(childRoomNode.id);

                        //remove parent id from the child room node
                        childRoomNode.RemoveParentoomNodeIdFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        ClearAllSelectedRoomNodes();
    }

    /// <summary>
    /// Delete selected room nodes
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodesToDelete = new Queue<RoomNodeSO>();

        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodesToDelete.Enqueue(roomNode);

                //iterate through child room nodes ids
                foreach (string childRoomNodeId in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeId);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentoomNodeIdFromRoomNode(roomNode.id);
                    }
                }

                //iterate through parent room nodes ids
                foreach (string parentRoomNodeId in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeId);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIdFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        while(roomNodesToDelete.Count > 0)
        {
            //get room node from queue
            RoomNodeSO roomNodeToDelete = roomNodesToDelete.Dequeue();

            //remove node from dictionary
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            //remove node from current room node graph list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            
            //remove node from asset database
            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }

        ClearAllSelectedRoomNodes();
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        //Loop through all the nodes and draw them
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(selectedRoomNodeStyle);
            }
            else if (!roomNode.isSelected)
            {
                roomNode.Draw(roomNodeStyle);
            }
            if (roomNode.roomNodeType.isEntrance)
            {
                roomNode.Draw(entraceRoomNodeStyle);
            }
            else if (roomNode.roomNodeType.isBossRoom)
            {
                roomNode.Draw(bossRoomNodeStyle);
            }
            else if (roomNode.roomNodeType.roomNodeTypeName.Contains("Chest Room"))
            {
                roomNode.Draw(chestRoomNodeStyle);
            }
        }

        GUI.changed = true; 
    }

    private void DrawGridLines(float gridSize, float opacity, Color color)
    {
        //position rect editor screen
        int numOfVerticalLinesToDraw = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int numOfHorizontalLinesToDraw = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(color.r, color.g, color.g, opacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        //start drawing from slightly beyond the screens
        for (int i  = 0; i < numOfVerticalLinesToDraw; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0f) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int i = 0; i < numOfHorizontalLinesToDraw; i++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0f) + gridOffset, new Vector3(position.width + gridSize, gridSize * i, 0f) + gridOffset);
        }

        Handles.color = Color.white;

    }

    /// <summary>
    /// Check to see if mouse is over a room node - if so then return the room node
    /// else return null
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for(int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition)){
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNodeSO in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNodeSO.isSelected)
            {
                roomNodeSO.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNodeSO in currentRoomNodeGraph.roomNodeList)
        {
            roomNodeSO.isSelected = true;
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Draw Connections in the graph between room nodes
    /// </summary>
    private void DrawRoomNodeConnections()
    {
        //loop through all room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.childRoomNodeIDList.Count > 0)
            {
                //Loop through all child room nodes

                foreach(string childRoomNodeId in roomNode.childRoomNodeIDList)
                {
                    //get child room node from dictionary
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeId))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeId]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        //main line start and end pos
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        //midpoint of the line
        Vector2 midPoint = (endPosition+startPosition)/2f;

        //direction of the line
        Vector2 direction = endPosition - startPosition;

        //calculate nomalized perpendicular positions from thhe midpoint
        //swap x and y values of direction and negate the y value
        Vector2 arrowTailPoint1 = midPoint - new Vector2(-direction.y, direction.x).normalized * connectingArrowSize;
        Vector2 arrowTailPoint2 = midPoint + new Vector2(-direction.y, direction.x).normalized * connectingArrowSize;

        //calculate midpoint offset for arrow head
        Vector2 arrowHeadPoint = midPoint + direction.normalized * connectingArrowSize;

        //draw arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        //draw main line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);


        GUI.changed = true;
    }

    /// <summary>
    /// Selection changed in editor
    /// </summary>
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }

    private void CountRoomTypes()
    {
        smallRoomsCount = 0;
        mediumRoomsCount = 0; 
        largeRoomsCount = 0;
        chestRoomsCount = 0;
        totalRoomsCount = 0;
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if(roomNode.roomNodeType.roomNodeTypeName.Contains("Small Room")){
                smallRoomsCount++;
            }
            else if (roomNode.roomNodeType.roomNodeTypeName.Contains("Medium Room")){
                mediumRoomsCount++;
            }
            else if(roomNode.roomNodeType.roomNodeTypeName.Contains("Large Room")){
                largeRoomsCount++;
            }
            else if (roomNode.roomNodeType.roomNodeTypeName.Contains("Chest Room"))
            {
                chestRoomsCount++;
            }
            else if (roomNode.roomNodeType.roomNodeTypeName.Contains("Boss Room"))
            {
                totalRoomsCount++;
            }
            else if (roomNode.roomNodeType.roomNodeTypeName.Contains("Entrance"))
            {
                totalRoomsCount++;
            }
        }
        totalRoomsCount = totalRoomsCount + smallRoomsCount + mediumRoomsCount + largeRoomsCount + chestRoomsCount;
    }
}
