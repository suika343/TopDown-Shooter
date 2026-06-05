using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MoveItem : MonoBehaviour
{
    #region Sound Effect
    [Header("SOUND EFFECT")]
    #endregion
    [SerializeField] private SoundEffectSO moveSoundEffect;

    [HideInInspector] public BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidbody2D;
    private InstantiatedRoom instantiatedRoom;
    private Vector3 previousPosition;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        instantiatedRoom = GetComponentInParent<InstantiatedRoom>();

        instantiatedRoom.moveableItemsList.Add(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    private void UpdateObstacles()
    {
        //make sure item stays in the room
        ConfineItemToRoomBounds();

        instantiatedRoom.UpdateMoveableObstacles();

        previousPosition = transform.position;

        if(Mathf.Abs(rigidbody2D.linearVelocity.x) > 0.1f || Mathf.Abs(rigidbody2D.linearVelocity.y) > 0.1f)
        {
            SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
        }
    }

    private void ConfineItemToRoomBounds()
    {
        
    }
}
