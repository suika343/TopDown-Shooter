using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region HEADER GAME OBJECT REFERENCES
    [Space(10)]
    [Header("Game Object References")]
    #endregion
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;

    #region Tooltip
    [Tooltip("Populate with PauseMenu UI GameObject")]
    #endregion
    [SerializeField] private GameObject pauseMenuUI;

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
    private bool isFading = false;


    [Space(10)]
    [Header("Test Code")]
    [Tooltip("If true, the dungeon will be completed automatically for testing purposes")]
    public bool testCodeCompleteDungeon = false;


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
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
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

    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
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
            case GameState.playingLevel:
                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.engagingEnemies:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.dungeonOverviewMap:
                if(Input.GetKeyUp(KeyCode.Tab))
                {
                    DungeonMap.Instance.ClearDungeonOverviewMap();
                }
                break;
            case GameState.bossStage:
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.engagingBoss:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
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
            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
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

        StartCoroutine(DisplayDungeonLevelText());

        //test code
        if (testCodeCompleteDungeon)
        {
            RoomEnemiesDefeated();
        }
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL: " + (currentDungeonLevelListIndex + 1) + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName;

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    private IEnumerator DisplayMessageRoutine(string messageText, Color textColor, float displayDuration)
    {
        //Set Text
        messageTextTMP.text = messageText;
        messageTextTMP.color = textColor;

        //Display Message for a given time
        //skip when player presses enter
        if(displayDuration > 0f)
        {
            float elapsedTime = displayDuration;
            while(elapsedTime > 0 && (!Input.GetKeyDown(KeyCode.Return)))
            {
                elapsedTime -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        //Clear text
        messageTextTMP.text = "";
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

    public void PauseGameMenu()
    {
        if(gameState != GameState.gamePaused)
        {
            previousGameState = gameState;
            gameState = GameState.gamePaused;

            GetPlayer().playerControl.DisablePlayer();
            pauseMenuUI.SetActive(true);
        }
        else
        {
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
            GetPlayer().playerControl.EnablePlayer();
            pauseMenuUI.SetActive(false);
        }

    }

    private void DisplayDungeonOverviewMap()
    {
        if (isFading)
            return;

        DungeonMap.Instance.DisplayDungeonOverviewMap();
    }

    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        //fade in canvas
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        //Display Boss Message
        yield return StartCoroutine(DisplayMessageRoutine("A HEAVY DOOR IS UNLOCKED... SOMEWHERE", Color.white, 5f));

        //fade in canvas
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        //fade in canvas
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0.65f, 0.25f, 0.25f, 0.4f)));

        //Display Level Complete Message
        yield return StartCoroutine(DisplayMessageRoutine("LEVEL COMPLETED!", Color.white, 5f));

        //fade in canvas
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0.65f, 0.25f, 0.25f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        //increase level and play next level
        currentDungeonLevelListIndex++; 
        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    public IEnumerator Fade(float startAlpha, float targetAlpha, float duration, Color fadeColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = fadeColor;

        float elapsedTime = 0f;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        isFading = false;
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        
        //Disable player
        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText = "";

        if(rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string currentPlayerName = GameResources.Instance.currentPlayer.playerName;

            if(currentPlayerName == "")
            {
                currentPlayerName = playerDetails.playerCharacterName.ToUpper();
            }

            //Update Score
            HighScoreManager.Instance.AddScore(new Score()
            {
                playerName = currentPlayerName,
                levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString()
                    + " " + GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        }
        else
        {
            rankText = "YOUR SCORE IS NOT RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        yield return new WaitForSeconds(1f);

        //fade in canvas
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        //Display Game Won Messages
        yield return StartCoroutine(DisplayMessageRoutine("VICTORY! " 
            + GameResources.Instance.currentPlayer.playerName
            + " YOU HAVE COMPLETED THE DUNGEON!", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("SCORE: " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO PLAY AGAIN", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        //Disable player
        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText = "";

        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string currentPlayerName = GameResources.Instance.currentPlayer.playerName;

            if (currentPlayerName == "")
            {
                currentPlayerName = playerDetails.playerCharacterName.ToUpper();
            }

            //Update Score
            HighScoreManager.Instance.AddScore(new Score()
            {
                playerName = currentPlayerName,
                levelDescription = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString()
                    + " " + GetCurrentDungeonLevel().levelName.ToUpper(),
                playerScore = gameScore
            }, rank);
        }
        else
        {
            rankText = "YOUR SCORE IS NOT RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }

        yield return new WaitForSeconds(1f);

        //fade in canvas
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        //Disable Enemies
        Enemy[] enemyArray = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        //Display Game Lost Messages
        yield return StartCoroutine(DisplayMessageRoutine("UNLUCKY... YOU LOST", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("SCORE: " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO PLAY AGAIN", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenuUI), pauseMenuUI);
    }
#endif
    #endregion
}
