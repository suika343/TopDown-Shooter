using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        //Load the Room Node Type List
        LoadRoomNodeTypeList();

        // Set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1.0f);
    }

    /// <summary>
    /// Load Rom Node Type List
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodetypeList;
    }

    /// <summary>
    /// Generate random dungeon, return true if dungeon is built false otherwise
    /// </summary>
    /// <param name="currentDungeonLevel"></param>
    /// <returns></returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplatesList;

        LoadRoomTemplateIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts <= Settings.maxDungeonRebuildAttempts)
        {
            dungeonBuildAttempts++;

            //Select a random room node graph from the list
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);
            
            int dungeonRebuildAttemptsForRoomGraph = 0;
            dungeonBuildSuccessful = false;

            //Loop until dungeon is successfully built or max attempts reached
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForRoomGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                //Clear the dungeon
                ClearDungeon();
                dungeonRebuildAttemptsForRoomGraph++;

                //Attemp to build the dungeon
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }
            if (dungeonBuildSuccessful)
            {
                //Instantiate the gameObjects
                //InstantiateRoomGameObjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplateIntoDictionary()
    {
        //Clear the dictionary
        roomTemplateDictionary.Clear();

        //Load room template list into dictionary
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key in " + roomTemplateList);
            }
        }
    }


    /// <summary>
    /// Attemp to build a random dungeon for the specified room node graph.
    /// return true if a successful dungeon layout is built, false if a problem occurs
    /// and the dungeon needs to be rebuilt
    /// </summary>
    /// <param name="roomNodeGraph"></param>
    /// <returns></returns>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        //Create a queue of room nodes to create
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        //Add the entrance node to the queue
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node in Room Node Graph");
            return false; //Dungeon not built
        }

        //Start with no room overlaps
        bool noRoomOverlaps = true;

        // Process open room node queue
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        //If all the room nodes in the queue are processed and there are no room overlaps
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            //Get next room node from the queue
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            //Add child room nodes of the room to the open room node queue
            foreach(RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            //if the room is the entrance mark as positioned and add to dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                //Add room to dictionary
                dungeonBuilderRoomDictionary.Add(roomNode.id, room);
            }
        }
        return noRoomOverlaps;
    }

    /// <summary>
    /// Get a random room template from the room template list that matches the room node type
    /// returns null if no matching template is found
    /// </summary>
    /// <param name="roomNodeType"></param>
    /// <returns></returns>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplates = new List<RoomTemplateSO>();

        //Loop through room templates and find the ones that match the room node type
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            //Add matching room templates
            if(roomTemplate.roomNodeType = roomNodeType)
            {
                matchingRoomTemplates.Add(roomTemplate);
            }
        }

        //return random room template from the matching list
        //else return null if there is no matching template
        if (matchingRoomTemplates.Count > 0)
        {
            return matchingRoomTemplates[Random.Range(0, matchingRoomTemplates.Count)];
        }
        else
        {
            Debug.Log("No Room Template found for Room Node Type " + roomNodeType.name);
            return null;
        }
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        //Initialize a new room from the room template
        Room room = new Room();
        
        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        //initially set room bounds to the room template bounds
        //but it will be updated when the room is positioned in the scene
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.GetDoorwayList());

        //Set Parent ID for room
        //If the room node has no parent room nodes then it is the entrance
        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            //player always starts in the entrance room
            room.isPreviouslyVisited = true;
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

            return room;
    }

    /// <summary>
    /// Select a random room node graph from the room node graph list
    /// </summary>
    /// <param name="roomNodeGraphList"></param>
    /// <returns></returns>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("No Room Node Graphs in the list");
            return null;
        }

    }


    /// <summary>
    /// Creates a new string List rather than reference to the List
    /// </summary>
    /// <param name="listToCopy"></param>
    /// <returns></returns>
    private List<string> CopyStringList(List<string> listToCopy)
    {
        List<string> newStringList = new List<string>();
        foreach (string item in listToCopy)
        {
            newStringList.Add(item);
        }
        return newStringList;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway); 
        }
        return newDoorwayList;
    }

    private void ClearDungeon()
    {
        //Destroy instantiated dungeon game objects and clear dungeon manager room dictionary
        if(dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();

        }
    }
}
