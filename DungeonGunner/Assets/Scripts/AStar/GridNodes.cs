using UnityEngine;
using System;

public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNodes;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridNodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition >= 0 && xPosition < width && yPosition >= 0 && yPosition < height)
        {
            return gridNodes[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Grid position out of bounds: (" + xPosition + ", " + yPosition + ")");
            return null;
        }
    }
}
