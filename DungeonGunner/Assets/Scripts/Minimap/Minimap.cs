using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the Minimap Player GameObject")]
    #endregion
    [SerializeField] 
    private GameObject minimapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        CinemachineVirtualCamera virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        virtualCamera.Follow = playerTransform;

        SpriteRenderer minimapPlayerSpriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if(minimapPlayerSpriteRenderer != null)
        {
            minimapPlayerSpriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
        }
    }

    private void Update()
    {
        if(playerTransform != null && minimapPlayer != null)
        {
            minimapPlayer.transform.position = playerTransform.position;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }
#endif
    #endregion
}
