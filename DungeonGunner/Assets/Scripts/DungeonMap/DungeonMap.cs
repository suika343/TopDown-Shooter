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
