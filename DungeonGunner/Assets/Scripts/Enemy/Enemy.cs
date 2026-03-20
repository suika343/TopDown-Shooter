using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
#endregion
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    private MaterializeEffect materializeEffect;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void InitializeEnemy(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;
        SetEnemyMovementUdpateFrame(enemySpawnNumber);
        SetEnemyAnimationSpeed();
        //MaterializeEnemy
        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyAnimationSpeed()
    {
        //Set the animation speed based on the move speed of the enemy
        float animationSpeed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
        animator.speed = animationSpeed;
    }

    private void SetEnemyMovementUdpateFrame(int enemySpawnNumber)
    {
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathFindingOver);
    }

    private IEnumerator MaterializeEnemy()
    {
        //Disable enemy collider and AI
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeCoroutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
            enemyDetails.enemyMaterializeEffectTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        //Enable enemy collider and AI
        EnemyEnable(true);
    }

    private void EnemyEnable(bool enable)
    {
        //Colliders
        circleCollider2D.enabled = enable;
        polygonCollider2D.enabled = enable;

        //Movement AI
        enemyMovementAI.enabled = enable;
    }
}
