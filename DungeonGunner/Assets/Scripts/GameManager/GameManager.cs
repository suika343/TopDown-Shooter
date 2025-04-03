using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [SerializeField]
    private GameState gameState;

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
