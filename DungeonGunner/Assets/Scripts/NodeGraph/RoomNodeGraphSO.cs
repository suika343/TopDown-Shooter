using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(fileName = "RoomNodeGraph_", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        foreach(RoomNodeSO roomNode in roomNodeList)
        {
            roomNodeDictionary[roomNode.id] = roomNode;
        }
    }

    #region EDITOR CODE
#if UNITY_EDITOR
    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawLineConnectionFrom(RoomNodeSO roomNode, Vector2 position)
    {
        roomNodeToDrawLineFrom = roomNode;
        linePosition = position; 
    }
#endif
    #endregion EDITOR CODE
}

