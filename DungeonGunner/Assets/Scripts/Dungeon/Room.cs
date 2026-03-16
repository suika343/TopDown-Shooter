using UnityEngine;
using System.Collections.Generic;

public class Room 
{
    public string id;
    public string templateID;
    public GameObject prefab;
    public RoomNodeTypeSO roomNodeType;
    //bounds when the room is placed
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;
    //bounds in the room template
    public Vector2Int templateLowerBounds;
    public Vector2Int templateUpperBounds;

    public Vector2Int[] spawnPositionArray;

    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomLevelEnemySpawnParametersList;

    public List<string> childRoomIDList;
    public string parentRoomID;
    public List<Doorway> doorwayList;
    public bool isPositioned = false;
    public InstantiatedRoom instantiatedRoom;

    public bool isLit = false;
    public bool isClearOfEnemies = false;
    public bool isPreviouslyVisited = false;

    public Room()
    {
        childRoomIDList = new List<string>();
        doorwayList = new List<Doorway>();
    }

    public int GetNumberOfEnemiesToSpawn(DungeonLevelSO dungeonLevel)
    {
        int numberOfEnemiesToSpawn = 0;
        foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (roomEnemySpawnParameters.dungeonLevel == dungeonLevel)
            {
                numberOfEnemiesToSpawn = Random.Range(roomEnemySpawnParameters.minTotalEnemiesToSpawn, roomEnemySpawnParameters.maxTotalEnemiesToSpawn + 1);
                break;
            }
        }
        return numberOfEnemiesToSpawn;
    }

    public RoomEnemySpawnParameters GetRoomEnemySpawnParameters(DungeonLevelSO dungeonLevel)
    {
        RoomEnemySpawnParameters roomEnemySpawnParameters = null;
        foreach (RoomEnemySpawnParameters roomLevelEnemySpawnParameters in roomLevelEnemySpawnParametersList)
        {
            if (roomLevelEnemySpawnParameters.dungeonLevel == dungeonLevel)
            {
                roomEnemySpawnParameters = roomLevelEnemySpawnParameters;
                break;
            }
        }
        return roomEnemySpawnParameters;
    }
}
