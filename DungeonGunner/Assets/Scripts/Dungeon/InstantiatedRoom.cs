using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        //get room collider bounds from box collider 2d
        roomColliderBounds = boxCollider2D.bounds;
    }

    /// <summary>
    /// Initialize the instantiated room with the room game object.
    /// </summary>
    /// <param name="roomGameObject"></param>
    public void Initialize(GameObject roomGameObject)
    {
        PopulateTilemapVariables(roomGameObject);
        BlockOffUnusedDoorways();
        AddDoorsToRooms();
        DisableCollisionTilemapRenderer();
    }

    private void PopulateTilemapVariables(GameObject roomGameObject)
    {
        //get the grid and tilemaps from the room game object
        grid = roomGameObject.GetComponentInChildren<Grid>();

        //Get tilemaps from children
        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if(tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// Blocks off unused doorways in the room.
    /// </summary>
    private void BlockOffUnusedDoorways()
    {
        //loop through all the doorways
        foreach(Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
                continue;

            //block off unconnected doorways using tiles on tilemaps
            if(collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block a doorway on the give tilemap layer.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }
    }

    /// <summary>
    /// for north and south dooways
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        //loop through all the tiles to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for(int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //get rotation of the tile to be copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                //copy tiles
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos + 1, startPosition.y - yPos, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //set rotation of the tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos + 1, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// for east and west doorways
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="doorway"></param>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        //loop through all the tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                //get rotation of the tile to be copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                //copy tiles
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos - 1, 0),
                    tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                //set rotation of the tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos - 1, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Add doors to rooms if this is not a corridor room
    /// </summary>
    private void AddDoorsToRooms()
    {
        // return if the room is a corridor
        if(room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS)
        {
            return;
        }
        //loop through each doorway in the room
        foreach (Doorway doorway in room.doorwayList)
        {
            //check uif the doorway is connected and if there is a door prefab
            if(doorway.isConnected && doorway.doorPrefab != null)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;
                //Create doorway with the current room as the parent
                if(doorway.orientation == Orientation.north)
                {
                    //create door gameObject
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    //manipulate the door local position to position it properly
                    //doorway prefab's door position for North is moved half a unit in x-axis and one unit in y-axis
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    //create door gameObject
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    //manipulate the door local position to position it properly
                    //doorway prefab's door position for South is moved half a unit in x-axis
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f,
                        doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    //create door gameObject
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    //manipulate the door local position to position it properly
                    //doorway prefab's door position for East is moved one a unit in x-axis and 1.25 units in the Y axis
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance,
                        doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    //create door gameObject
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    //manipulate the door local position to position it properly
                    //doorway prefab's door position for West is moved 1 unit * 1.25 units in the Y axis
                    door.transform.localPosition = new Vector3(doorway.position.x,
                        doorway.position.y + tileDistance * 1.25f, 0f);
                }

                //Get Door Component
                Door doorComponent = door.GetComponent<Door>();
                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    //Lock boss room until the dungeon is cleared
                    doorComponent.LockDoor();
                }
            }
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        if (collisionTilemap != null)
        {
            collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
        }
    }
}
