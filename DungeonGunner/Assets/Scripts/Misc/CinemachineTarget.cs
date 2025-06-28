using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineTarget))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;
    [Tooltip("Populate with the CursorTarget GameObject")]
    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
        {
            Object = GameManager.Instance.GetPlayer().transform,
            Weight = 1,
            Radius = 2.5f
        };

        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
        {
            Object = cursorTarget,
            Weight = 1,
            Radius = 1f
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

        // Updated to use the 'Targets' property instead of the obsolete 'm_Targets' field
        cinemachineTargetGroup.Targets = new System.Collections.Generic.List<CinemachineTargetGroup.Target>(cinemachineTargetArray);
    }
}
