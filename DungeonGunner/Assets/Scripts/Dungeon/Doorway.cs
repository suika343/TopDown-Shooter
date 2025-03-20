using UnityEngine;
using System;

[System.Serializable]
public class Doorway
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;

    #region Header
    [Header("The Upper Left Position to Start Copying From")]
    #endregion 
    public Vector2Int doorWayStartCopyPosition;

    #region Header
    [Header("The width of the tiles in the doorway to copy over")]
    #endregion
    public Vector2Int doorWayCopyTileWidth;

    #region Header
    [Header("The height of the tiles in the doorway to copy over")]
    #endregion
    public Vector2Int doorWayCopyTileHeight;

    [HideInInspector]
    public bool isConnected = false;
    [HideInInspector]
    public bool isUnavailable = false;

}
