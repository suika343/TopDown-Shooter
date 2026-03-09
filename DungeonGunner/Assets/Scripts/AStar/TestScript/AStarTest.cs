using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPosition;
    private Vector3Int endGridPosition;
    private TileBase startPathTile;
    private TileBase endPathTile;

    private Vector3Int noValue =  new Vector3Int(999, 999, 999);
    private Stack<Vector3> pathStack;

    private void Start()
    {
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        endPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    private void Update()
    {
        if(instantiatedRoom == null || pathTilemap == null || grid == null || startPathTile == null || endPathTile == null)
        {
            Debug.Log("AStarTest: Update: Missing references, cannot run A* test");
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void SetStartPosition()
    {
        if(startGridPosition == noValue)
        {
            startGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if(!IsPositionWithinBounds(startGridPosition))
            {
                startGridPosition = noValue;
                Debug.Log("AStarTest: SetStartPosition: Start position is out of bounds");
                return;
            }

            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBounds(endGridPosition))
            {
                endGridPosition = noValue;
                Debug.Log("AStarTest: SetStartPosition: Start position is out of bounds");
                return;
            }

            pathTilemap.SetTile(endGridPosition, endPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    private bool IsPositionWithinBounds(Vector3Int gridPosition)
    {
        if(gridPosition.x < instantiatedRoom.room.templateLowerBounds.x 
            || gridPosition.x > instantiatedRoom.room.templateUpperBounds.x
            || gridPosition.y < instantiatedRoom.room.templateLowerBounds.y
            || gridPosition.y > instantiatedRoom.room.templateUpperBounds.y)
        {
            return false;
        }
        return true;
    }

    private void ClearPath()
    {
        if(pathStack == null)
            return;

        foreach(Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    private void DisplayPath()
    {
        if(startGridPosition == noValue || endGridPosition == noValue)
        {
            Debug.Log("AStarTest: DisplayPath: Start or end position is not set");
            return;
        }
        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);
        
        if(pathStack == null)
        {
            Debug.Log("AStarTest: DisplayPath: Could not build path");
            return;
        }

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
        }
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        //ClearPath();
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.frontTilemap;
        grid = instantiatedRoom.grid;
        startGridPosition = noValue;
        endGridPosition = noValue;

        //duplicate the front tilemap to create a path tilemap
        SetUpPathTileMap();
    }

    private void SetUpPathTileMap()
    {
        //check if there is clone, if there is, reuse the clone, if not, create a new one
        Transform tilemapCloneTransform = frontTilemap.transform.parent.Find("PathTilemap");

        if(tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
            pathTilemap.gameObject.name = "PathTilemap";
        }
        else
        {
            frontTilemap.transform.parent.Find("PathTilemap");
            pathTilemap.ClearAllTiles();
        }
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }
}
