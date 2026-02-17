using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header - OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion

    #region Tooltip
    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameObject")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    //lock the opened door once player enters and has enemies
    private bool isPreviouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        //disable door collider by default
        doorCollider.enabled = false;

        //load components
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag || collision.tag == Settings.playerWeaponTag)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // When the parent gameObject is disabled (when the player moves away too far from the room)
        // the animator state gets reset. This is to restore the animator state when re-enabling the parent gameObject.
        animator.SetBool(Settings.open, isOpen);
    }

    /// <summary>
    /// Open the Door
    /// </summary>
    private void OpenDoor()
    {
        if (isOpen)
        {
            return;
        }
        isOpen = true;
        isPreviouslyOpened = true;
        doorCollider.enabled = false;
        doorTrigger.enabled = false;
        animator.SetBool(Settings.open, true);
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
    }

    /// <summary>
    /// Lock the door
    /// </summary>
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;
        //set open to close to play closing animation
        animator.SetBool(Settings.open, false);
    }

    /// <summary>
    /// Unlock the door
    /// </summary>
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (isPreviouslyOpened)
        {
            isOpen = false;
            OpenDoor();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
}
