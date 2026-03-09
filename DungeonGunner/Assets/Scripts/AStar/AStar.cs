using UnityEngine;
using System.Collections.Generic;
using System.Data;

public static class AStar
{
    /// <summary>
    /// Builds a path for the room from the start grid position to the end grid position and adds
    /// movement steps to the returned Stack. Returns null if no path can be found.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        //Adjust start and end grid positions to be relative to the room template's lower bounds
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        //Build Open List and Closed HashSet
        List<Node> openNodesList = new List<Node>();
        HashSet<Node> closedNodesHashSet = new HashSet<Node>();

        //Create grid nodes for the room template
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodesList, closedNodesHashSet, room.instantiatedRoom);

        if(endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }
        return null; 
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodesList, 
        HashSet<Node> closedNodesHashSet, InstantiatedRoom instantiatedRoom)
    {
        //Add start node to the open nodes list
        openNodesList.Add(startNode);
        //Loop through the open nodes list until there are no more nodes to check
        while (openNodesList.Count > 0)
        {
            //Sort open nodes list with the lowest FCost
            openNodesList.Sort();

            //Get the node with the lowest FCost and remove it from the open nodes list
            Node currentNode = openNodesList[0];
            openNodesList.Remove(currentNode);

            //if currentNode is targetNode, return currentNode
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            //Add current node to the closed nodes hash set
            closedNodesHashSet.Add(currentNode);

            //Evaulaute all the neighboring nodes of the current node
            EvaluateCurrentNodeNeighbors(currentNode, targetNode, gridNodes, openNodesList, closedNodesHashSet, instantiatedRoom);
        }

        return null;
    }

    public static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        //Create path stack by backtracking from the target node to the start node using the parent nodes of each node
        //Stack = LIFO
        Stack<Vector3> pathStack = new Stack<Vector3>();
        Node nextNode = targetNode;

        //Get mid point of cell for each node
        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize / 2f;
        cellMidPoint.z = 0;

        while (nextNode != null)
        {
            //Convert grid position back to world position and add to path stack
            //Add the room template's lower bounds to the node's grid position to get the correct world position within the room
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x,
                nextNode.gridPosition.y + room.templateLowerBounds.y, 0));
            worldPosition += cellMidPoint;

            pathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return pathStack;
    }

    private static void EvaluateCurrentNodeNeighbors(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodesList,
        HashSet<Node> closedNodesHashSet, InstantiatedRoom instantiatedRoom)
    {
        //position of the current node in grid coordinates`
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighborNode;

        //loop through all the neighboring nodes of the current node (including diagonals)
        for (int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                //skip current node
                if(i == 0 && j == 0)
                {
                    continue;
                }

                validNeighborNode = GetValidNodeNeighbor(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes, closedNodesHashSet, instantiatedRoom);

                if(validNeighborNode != null)
                {
                    //Calculate new gCost
                    int newCostToNeighborNode;

                    //Get the movement penalty 
                    //Unwalkable paths have a value of 0. Default movement penalty
                    //is set in Settings file and applies to other grid squares.
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighborNode.gridPosition.x, validNeighborNode.gridPosition.y]; 

                    newCostToNeighborNode = currentNode.gCost + GetDistanceBetweenNodes(currentNode, validNeighborNode) + movementPenaltyForGridSpace;

                    bool isValidNeighborNodeInOpenNodesList = openNodesList.Contains(validNeighborNode);

                    //recalculated fCost is lower or the node is not yet in the open nodes list, update the node's gCost, hCost, and parent node
                    if (newCostToNeighborNode < validNeighborNode.gCost || !isValidNeighborNodeInOpenNodesList)
                    {
                        validNeighborNode.gCost = newCostToNeighborNode;
                        validNeighborNode.hCost = GetDistanceBetweenNodes(validNeighborNode, targetNode);
                        validNeighborNode.parentNode = currentNode;

                        if(!isValidNeighborNodeInOpenNodesList)
                        {
                            openNodesList.Add(validNeighborNode);
                        }
                    }
                }
            }
        }
    }

    public static int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        //10 units for horizontal/vertical movement, 14 units for diagonal movement (based on Pythagorean theorem)
        if (xDistance > yDistance)
        {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        }
        else
        {
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
    }

    private static Node GetValidNodeNeighbor(int neightborNodeXPosition, int neightborNodeYPosition, 
        GridNodes gridNodes, HashSet<Node> closedNodesHashSet, InstantiatedRoom instantiatedRoom)
    {
        //check if neighbor node is within grid bounds, return null if neighbor node is out of bounds
        if (neightborNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x
            || neightborNodeXPosition  < 0 || neightborNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y
            || neightborNodeYPosition < 0)
        {
            return null;
        }

        //Get Neighbor Node
        Node neighborNode = gridNodes.GetGridNode(neightborNodeXPosition, neightborNodeYPosition);

        //Check if Neighbor Node movement penalty is 0
        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neightborNodeXPosition, neightborNodeYPosition];

        //Skip if node is already in the closed nodes hash set or it is an obstacle
        if (closedNodesHashSet.Contains(neighborNode) || movementPenaltyForGridSpace == 0)
        {
            return null;
        }

        return neighborNode;
    }
}
