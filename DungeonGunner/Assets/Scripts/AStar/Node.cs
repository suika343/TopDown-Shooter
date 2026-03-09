using UnityEngine;
using System;
public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;

    public int gCost = 0; // the cost from the start node to this node
    public int hCost = 0; // the cost from this node to the end node
    
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }
    public int FCost 
    {         
        get
        {
            return gCost + hCost;
        }
    }

    public Node parentNode;
    public int CompareTo(Node nodeToCompare)
    {
        //compare will be based on the fCost of the node, if the fCost is the same then it will compare the hCost
        //0 if the values are the same, -1 if this node is smaller than the node to compare and 1 if this node is bigger than the node to compare

        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}
