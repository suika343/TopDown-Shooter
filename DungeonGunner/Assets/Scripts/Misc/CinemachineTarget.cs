using UnityEngine;
using Cinemachine;

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
            target = GameManager.Instance.GetPlayer().transform,
            weight = 1,
            radius = 2.5f
        };

        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
        {
            target = cursorTarget,
            weight = 1,
            radius = 1f
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };

        // Updated to use the 'Targets' property instead of the obsolete 'm_Targets' field
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }
}
