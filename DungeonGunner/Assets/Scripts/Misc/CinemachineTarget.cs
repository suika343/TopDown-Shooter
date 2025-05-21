using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineTarget))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
        {
            Object = GameManager.Instance.GetPlayer().transform,
            Weight = 1,
            Radius = 1f
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTarget_player };

        // Updated to use the 'Targets' property instead of the obsolete 'm_Targets' field
        cinemachineTargetGroup.Targets = new System.Collections.Generic.List<CinemachineTargetGroup.Target>(cinemachineTargetArray);
    }
}
