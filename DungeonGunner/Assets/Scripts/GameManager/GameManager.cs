using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{

    #region HEADER DUNGEON LEVELS
    [Space(10)]
    [Header("Dungeon Levels")]
    #endregion

    #region Tooltip
    [Tooltip("List of all the dungeon levels in the game, populate with Dungeon Level Scriptable Objects")]
    #endregion
    [SerializeField]
    private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with starting level for testing, first level = 0")]
    #endregion
    [SerializeField]
    private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [SerializeField]
    private GameState gameState;

    protected override void Awake()
    {
        base.Awake();

        // Set player details from GameResources
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        //Instantiate Player
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        //Instantiate player prefab in the scene
        GameObject playerPrefab = Instantiate(playerDetails.playerPrefab);

        //Initialize player
        player = playerPrefab.GetComponent<Player>();

        player.InitializePlayer(playerDetails);
    }

    private void OnEnable()
    {
        //Subscribe to static event handler
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        //Subscribe to static event handler
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        //testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    private void HandleGameState()
    {
        switch(gameState)
        {
            case GameState.gameStarted:
                Debug.Log("Game Started");
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                break; 
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        DungeonLevelSO dungeonLevel = dungeonLevelList[dungeonLevelListIndex];
        Debug.Log("Playing Dungeon Level: " + dungeonLevel.levelName);
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevel);
        if(!dungeonBuiltSuccessfully) 
        {
            Debug.LogError("DUNGEON NOT BUILT - COULD NOT BUILD DUNGEON FROM SPECIFIED NODE GRAPHS");
        }
        else
        {
            Debug.Log("Dungeon Built Successfully!");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set player position roughly mmidway across the room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // Set the player position to the nearest spawn point
        player.gameObject.transform.position = HelperUtilities.GetSpawnPointNearestToPlayer(player.gameObject.transform.position);
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public Player GetPlayer()
    {
        return player;
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
