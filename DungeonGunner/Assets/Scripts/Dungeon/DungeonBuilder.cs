using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Drawing.Text;

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
            //If the dungeon is successfully built then instantiate the game objects
            if (dungeonBuildSuccessful)
            {
                //Instantiate the gameObjects
                InstantiateRoomGameObjects();
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
            //if the room is not an entrance
            else
            {
                //Get parent room for node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                //See if room can be places without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);

            }
        }
        return noRoomOverlaps;
    }

    /// <summary>
    /// returns true if room can be placed without overlaps, otherwise return false
    /// </summary>
    /// <param name="roomNode"></param>
    /// <param name="parentRoom"></param>
    /// <returns></returns>
    private bool CanPlaceRoomWithNoOverlaps (RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        //try to place against all available doorways of the parent
        //until the room is successfully place without overlaps
        while (roomOverlaps)
        {
            //Selecr random unconnected doorway from the parent room
            List<Doorway> unconnectedDoorwayList = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if(unconnectedDoorwayList.Count == 0)
            {
                //if no more doorways to connect to then return false
                //as it is an overlap failure
                return false;
            }

            Doorway parentDoorway = unconnectedDoorwayList[Random.Range(0, unconnectedDoorwayList.Count)];

            //Get a random room template for that is consistent with the doorway orientation
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, parentDoorway);

            //Create a room
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // try to place the room with no overlaps
            //returns true if there is no overlap
            if(PlaceTheRoom(parentRoom, parentDoorway, room))
            {
                //if room does not overlap
                roomOverlaps = false;

                //mark the room as positioned   
                room.isPositioned = true;

                //Add the room to the dictionary
                dungeonBuilderRoomDictionary.Add(roomNode.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        //no room overlaps
        return true;
    } 

    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway parentDoorway)
    {
        RoomTemplateSO roomTemplate = null;

        //If room node is a  corridor then select a random correct
        //corridor template for the parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (parentDoorway.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }

        }
        else
        {
            //Get a random room template for the room node type
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        } 

        return roomTemplate;

    }

    /// <summary>
    /// returns true if there are no overlaps
    /// </summary>
    /// <param name="parentRoom"></param>
    /// <param name="parentDoorway"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool PlaceTheRoom(Room parentRoom, Doorway parentDoorway, Room roomToPlace)
    {
        //Get current room doorway position - opposite room of the parent doorway
        Doorway currenRoomDoorway = GetOppositeDoorway(parentDoorway, roomToPlace.doorwayList);

        if(currenRoomDoorway == null)
        {
            Debug.Log("No Opposite Doorway found for " + parentDoorway.orientation);
            //set doorway as unavailable
            parentDoorway.isUnavailable = true;
            return false;
        }

        //Calculate world grid position of the parent doorway
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + parentDoorway.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        //Calculate the adjustment to the room position based on the parent doorway orientation that we are trying to connect
        //(e.g. if this dooorway is west then we need to add (1,0) to the east parent doorway
        switch (currenRoomDoorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }

        //calculate room lower bounds and upper bounds based on positioning to align to parent doorway
        roomToPlace.lowerBounds = parentDoorwayPosition + adjustment + roomToPlace.templateLowerBounds - currenRoomDoorway.position;
        roomToPlace.upperBounds = roomToPlace.lowerBounds + (roomToPlace.templateUpperBounds - roomToPlace.templateLowerBounds);

        Room overlappingRoom = CheckForRoomOverlaps(roomToPlace);

        if (overlappingRoom == null)
        {
            //mark doorways as connected
            parentDoorway.isConnected = true;
            parentDoorway.isUnavailable = true;
            currenRoomDoorway.isConnected = true;
            currenRoomDoorway.isUnavailable = true;

            return true;
        }
        else
        {
            //mark parent doorway as unavailable so it wont get checked again
            parentDoorway.isUnavailable = true;
            return false;
        }
    }

    /// <summary>
    /// Check for rooms that are overlapping the upper bounds and lower bounds of the room to check
    /// if there are overlapping rooms, return the first room found else return null
    /// </summary>
    /// <param name="roomToCheck"></param>
    /// <returns></returns>
    private Room CheckForRoomOverlaps(Room roomToTest)
    {
        //Loop through all the rooms in the dungeon
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            //Skip if it is the same room or if the room is not positioned
            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue; 
            }

            if(IsOverlappingRoom(roomToTest, room))
            {
                //If there is an overlap then return the room
                return room;
            }
        }
        //If no overlaps then return null
        return null;
    }

    /// <summary>
    /// Check if two rooms overlap each other or not
    /// </summary>
    /// <param name="roomToTest"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingIntervals(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverlappingIntervals(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);
        if (isOverlappingX && isOverlappingY)
        {
            //If there is an overlap then return true
            return true;
        }
        else 
        {
            return false;
        }
    }

    /// <summary>
    /// Use intervals to check if the rooms are overlapping
    /// if the largest of min1 and min2 is less than or equal to the smallest of max1 and max2 then the intervals are overlapping
    /// check for intervals on both axes and if there are both overlaps, then the rooms are overlapping
    /// </summary>
    /// <param name="minX1"></param>
    /// <param name="minX2"></param>
    /// <param name="maxX1"></param>
    /// <param name="maxX2"></param>
    /// <returns></returns>
    private bool IsOverlappingIntervals(int min1, int max1, int min2, int max2)
    {
        if (Mathf.Max(min1, min2) <= Mathf.Min(max1, max2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> roomDoorwayList)
    {
        //Get the opposite doorway for the parent doorway
        foreach (Doorway doorwayToCheck in roomDoorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
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
    /// Get Unconnected Doorways
    /// </summary>
    /// <param name="roomDoorwayList"></param>
    /// <returns></returns>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
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

    public Room GetRoom(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            Debug.Log("No Room found for ID " + roomID);
            return null;
        }
    }

    private RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if(roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            Debug.Log("No Room Template found for ID " + roomTemplateID);
            return null;
        }
    }

    /// <summary>
    /// Instantiate the room game objects in the scene using the room prefabs
    /// </summary>
    private void InstantiateRoomGameObjects()
    {

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
