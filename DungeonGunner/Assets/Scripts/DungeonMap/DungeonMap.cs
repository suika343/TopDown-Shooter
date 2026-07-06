using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    #region OBJECT REFERENCES
    [Space(10)]
    [Header("GameObject References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the MinimapUI GameObect")]
    #endregion
    [SerializeField] private GameObject minimapUI;
    private Camera dungeonMapCamera;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        //populate player as cinemachine target
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        //get dungeon map camera
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        //if mouse button is pressed and game state is dungeon overview map, get the room clicked to teleport to
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    private void GetRoomClicked()
    {
        //convert screen position to world position
        Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);

        //set z position to 0
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

        //check for colliders at cursor position
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        //check if any of the colliders are a room
        foreach(Collider2D collider2D in collider2DArray)
        {
            if(collider2D.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();

                //teleport player to room if it is clear of enemies and has been previously visited
                if (instantiatedRoom.room.isClearOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    //move player to room
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        StaticEventHandler.CallRoomChangedEvent(room);

        //faade out screen to black
        yield return StartCoroutine(GameManager.Instance.Fade(0f, 1f, 0f, Color.black));

        ClearDungeonOverviewMap();

        //disable player
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Get spawn point in room nearest to player
        Vector3 spawnPosition = HelperUtilities.GetSpawnPointNearestToPlayer(worldPosition);

        //move player
        GameManager.Instance.GetPlayer().transform.position = spawnPosition;

        //fade screen back in
        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        //enable player
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
    }

    public void DisplayDungeonOverviewMap()
    {
        //set game state
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        //Disable player
        GameManager.Instance.GetPlayer().playerControl.DisablePlayer();

        //Disable main camera and enable dungeon map camera
        mainCamera.gameObject.SetActive(false);
        dungeonMapCamera.gameObject.SetActive(true);

        //Ensure all rooms are active so they can be displayed
        ActivateRoomsForDisplay();

        //Disable minimap UI
        minimapUI.SetActive(false);
    }

    public void ClearDungeonOverviewMap()
    {
        //set game state to the previous game state
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        //Enable player
        GameManager.Instance.GetPlayer().playerControl.EnablePlayer();

        //Disable main camera and enable dungeon map camera
        dungeonMapCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        //Disable minimap UI
        minimapUI.SetActive(true);
    }

    private void ActivateRoomsForDisplay()
    {
        foreach(KeyValuePair<string, Room> roomKeyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = roomKeyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}
