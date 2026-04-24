using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;

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
    private InstantiatedRoom bossRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    //[HideInInspector]
    public GameState gameState;
    //[HideInInspector]
    public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;


    protected override void Awake()
    {
        base.Awake();

        // Set player details from GameResources
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        //Instantiate Player
        InstantiatePlayer();

        gameScore = 0;
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

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        //Subscribe to static event handler
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs eventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs eventArgs)
    {
        RoomEnemiesDefeated();
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs args)
    {
        gameScore += args.points * scoreMultiplier;

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnMultiplier(MultiplierArgs args)
    {
        if(args.multiplier)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier = 1;
        }

        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameState = GameState.gameStarted;
        previousGameState = GameState.gameStarted;
        scoreMultiplier = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        //testing
        if (Input.GetKeyDown(KeyCode.Numlock))
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
                RoomEnemiesDefeated();
                break;
            case GameState.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;
            case GameState.gameWon:
                if(previousGameState != GameState.gameWon)
                {
                    StartCoroutine(GameWon());
                }
                break;
            case GameState.gameLost:
                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines();
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.restartGame:
                RestartGame();
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

        // Set player position roughly midway across the room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // Set the player position to the nearest spawn point
        player.gameObject.transform.position = HelperUtilities.GetSpawnPointNearestToPlayer(player.gameObject.transform.position);

        //test code
        //RoomEnemiesDefeated();
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfEnemies = true;
        bossRoom = null;

        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            
            if(room.roomNodeType.isBossRoom)
            {
                bossRoom = room.instantiatedRoom;
                continue;
            }

            if (!room.isClearOfEnemies)
            {
                isDungeonClearOfEnemies = false;
                break;
            }
        }

        if((isDungeonClearOfEnemies && bossRoom == null) || (isDungeonClearOfEnemies && bossRoom.room.isClearOfEnemies))
        {   
            if(currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if(isDungeonClearOfEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
    }

    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        Debug.Log("Boss Stage Started");
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        Debug.Log("Level Completed - Press Enter to Continue");

        while(!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        //increase level and play next level
        currentDungeonLevelListIndex++; 
        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        Debug.Log("Game Won - All Levels Completed! Press Enter to Restart Game");
        while (!Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            yield return null;
        }
        yield return null;

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;
        Debug.Log("Game Lost - unlucky! Press Enter to Restart Game");
        while (!Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            yield return null;
        }
        yield return null;

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMinimapIcon()
    {
        return playerDetails.playerMinimapIcon;
    }

    public DungeonLevelSO GetDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
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
