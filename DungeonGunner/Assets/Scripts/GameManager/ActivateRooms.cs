using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    #region HEADER POPULATE WITH MINIMAP CAMERA
    [Header("POPULATE WITH MINIMAP CAMERA")]
    #endregion
    [SerializeField] private Camera minimapCamera;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating("EnableRooms", 0.5f, 1f);
    }

    private void EnableRooms()
    {
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int minimapCameraWorldPositionLowerBounds,
            out Vector2Int minimapCameraWorldPositionUpperBounds, minimapCamera);

        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds,
            out Vector2Int mainCameraWorldPositionUpperBounds, mainCamera);

        //loop through each room
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            //check if retrieved room is within the camera viewport, if it is, enable the room, else, disable
            if((room.lowerBounds.x <= minimapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= minimapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= minimapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= minimapCameraWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true);

                // check if room is within main camera viewport
                if (room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y &&
                    room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y)
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObjects();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapCamera), minimapCamera);
    }
#endif
    #endregion
}


