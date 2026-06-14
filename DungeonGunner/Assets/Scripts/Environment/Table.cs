using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Table : MonoBehaviour, IUsable
{
    #region Tooltip
    [Tooltip("Populate with item mass to be used in the rigidbody component")]
    #endregion
    [SerializeField]private float itemMasss;
    private bool itemUsed = false;

    private Animator animator;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void Use()
    {
        if(!itemUsed)
        {
            //Get collider bounds
            Bounds bounds = boxCollider2D.bounds;

            //Calculate closest point to player on collider bounds
            Vector3 closestPointToPlayer = bounds.ClosestPoint(GameManager.Instance.GetPlayer().GetPlayerPosition());

            //if player is closer to the right side of the table, flip the table left
            if (closestPointToPlayer.x == bounds.max.x)
            {
                animator.SetBool(Settings.flipLeft, true);
            }
            //if player is closer to the left side of the table, flip the table right
            else if (closestPointToPlayer.x == bounds.min.x)
            {
                animator.SetBool(Settings.flipRight, true);
            }
            //if player is closer to the bottom side of the table, flip the table up
            else if (closestPointToPlayer.y == bounds.min.y)
            {
                animator.SetBool(Settings.flipUp, true);
            }
            //if player is closer to the top side of the table, flip the table down
            else if (closestPointToPlayer.y == bounds.max.y)
            {
                animator.SetBool(Settings.flipDown, true);
            }
            else
            {
                animator.SetBool(Settings.flipDown, true);
            }

            //set layer to environment - bullets will now collide with the table
            gameObject.layer = LayerMask.NameToLayer("Environment");

            //set mass to specified value
            rigidBody2D.mass = itemMasss;

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlipSoundEffect);

            itemUsed = true;
        }
    }
    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(itemMasss), itemMasss, false);
    }
#endif
#endregion

}

