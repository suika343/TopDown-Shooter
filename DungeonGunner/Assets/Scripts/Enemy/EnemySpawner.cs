using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int numberOfEnemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawned;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawned = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // dont spawn enemies in corridors or entrance rooms
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
        {
            return;
        }
        // dont spawn enemies if the room has been cleared of enemies
        if (currentRoom.isClearOfEnemies)
        {
            return;
        }

        //get values from Room class
        numberOfEnemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetDungeonLevel());

        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetDungeonLevel());

        //return and mark room as clear of enemies if there are no enemies to spawn
        if (numberOfEnemiesToSpawn == 0)
        {
            currentRoom.isClearOfEnemies = true;
            return;
        }

        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        //lock doors
        currentRoom.instantiatedRoom.LockDoors();

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        //set game state to engaging enemies if we are currently playing the level
        if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        Grid grid = currentRoom.instantiatedRoom.grid;

        //Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        //check if there are spawn locations
        if(currentRoom.spawnPositionArray.Length > 0)
        {
            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                //wait until current enemy count is less than the max concurrent spawn number before spawning another enemy
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                //get random spawn position
                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    private float GetEnemySpawnInterval()
    {
        float spawnInterval = 0f;
        spawnInterval = Random.Range(roomEnemySpawnParameters.minimumSpawnInterval, roomEnemySpawnParameters.maximumSpawnInterval);
        return spawnInterval;
    }

    private int GetConcurrentEnemies() {
        int concurrentEnemies = 0;
        concurrentEnemies = Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies + 1);
        return concurrentEnemies;
    }   

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 spawnPosition)
    {
        //increment current enemy count and total enemies spawned
        currentEnemyCount++;
        enemiesSpawned++;

        DungeonLevelSO dungeonLevel = GameManager.Instance.GetDungeonLevel();

        //instantiate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, spawnPosition, Quaternion.identity, transform);

        //initialize enemy
        enemy.GetComponent<Enemy>().InitializeEnemy(enemyDetails, enemiesSpawned, dungeonLevel);

        //subscribe to enemy destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
    }

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        //unsubscribe from event
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
        //decrement current enemy count
        currentEnemyCount--;
        //check if all enemies have been spawned and defeated to mark room as clear of enemies
        if (enemiesSpawned == numberOfEnemiesToSpawn && currentEnemyCount <= 0)
        {
            currentRoom.isClearOfEnemies = true;

            //set game state
            if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.previousGameState = GameState.engagingBoss;
            }


            //unlock doors
            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            //call static event to notify that room enemies have been defeated
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }
}
