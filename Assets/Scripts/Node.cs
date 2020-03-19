using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool isWalkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;

    public int xGridPos;
    public int yGridPos;
    public Node parent;
    int heapIndex;

    public int fCost { get { return gCost + hCost; } }

    public Node(bool _walkable, Vector3 _worldPosition, int _x, int _y)
    {
        isWalkable = _walkable;
        worldPosition = _worldPosition;
        xGridPos = _x;
        yGridPos = _y;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node n)
    {
        int compare = fCost.CompareTo(n.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(n.hCost);
            
        }
        return -compare;
    }
}
