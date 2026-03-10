using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with MovementDetails Scriptable Object for of the enemy")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = true;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        //Create waitForFixedUpdate to be used in the Coroutine
        waitForFixedUpdate = new WaitForFixedUpdate();

        // Reset player reference position
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        //Check distance between player and enemy to see if the enemy should chase the player
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        // Exit if enemy is not close enough to the player to chase
        if (!chasePlayer)
        {
            return;
        }

        //if the movement cooldown timer has finished or the player has moved more than the require distance
        // then rebuild the path to the player and move the enemy
        if (currentEnemyPathRebuildCooldown <= 0f || Vector3.Distance(transform.position, 
            GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath)
        {
            // reset build path cooldown timer
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            // refresh player reference position
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            //Move enemy to player position using A Star pathfinding
            CreatePath();

            if(movementSteps != null)
            {
                if(moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        //get grid from instantiated room
        Grid grid = currentRoom.instantiatedRoom.grid;

        //get player grid position
        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        //get enemy grid position
        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // remove the first step in the movement steps stack because it is the enemy's current position
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }

    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        Vector3Int playerGridPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerPosition = new Vector2Int(playerGridPosition.x - currentRoom.templateLowerBounds.x, 
                playerGridPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerPosition.x, adjustedPlayerPosition.y];

        //return player position if the cell is not an obstacle cell
        if(obstacle != 0)
        {
            return playerGridPosition;
        }
        //else, find a surrounding cell that is not an obstacle cell and return that cell as the player grid position
        else
        {
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }
                    try
                    {
                        obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerPosition.x + i, adjustedPlayerPosition.y];
                        if(obstacle != 0)
                        {
                            return new Vector3Int(playerGridPosition.x + i, playerGridPosition.y + j, 0);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            // no other non obstacle cells found around the player, return the original player grid position (which is an obstacle cell)
            return playerGridPosition;
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 targetPosition = movementSteps.Pop();

            //move the enemy while the distance between the enemy and the target position is greater than a very small threshold
            while (Vector3.Distance(transform.position, targetPosition) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, transform.position, moveSpeed,
                    (targetPosition - transform.position.normalized));
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent();
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails); 
    }
#endif
    #endregion
}


