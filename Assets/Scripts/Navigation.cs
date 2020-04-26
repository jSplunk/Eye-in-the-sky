using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

/*
 * 
 *  Implementation based on Sebastian Lague
 *  'https://github.com/SebLague/Pathfinding'
 *
 */

 //Navigation class for both A* and JPS+
public class Navigation : MonoBehaviour
{
    

    [HideInInspector]
    public bool pathPending;

    public bool drawPath;
    public float remainingDistance;

    public List<Node> fullPath;

    Grid m_grid;
    Vector3 m_destination;

    void Awake()
    {
        m_grid = FindObjectOfType<Grid>();
        fullPath = new List<Node>();
        
    }

    public IEnumerator SetDestination(Vector3 Start,Vector3 Destination)
    {
        
        m_destination = Destination;
        Node startNode = m_grid.NodeFromWorldPoint(Start);
        Node destinationNode = m_grid.NodeFromWorldPoint(Destination);
        

        Queue<Node> open = new Queue<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        open.Enqueue(startNode);

        pathPending = true;

        while(open.Count > 0)
        {
            Node current = open.Dequeue();

            closed.Add(current);

            remainingDistance = m_grid.GetDistance(current, destinationNode);

            if (remainingDistance == 0)
            {
                Trace(startNode, destinationNode);
                yield return null;
            }


            /*
             * 
             *  JPS+ implementation (uncomment the GetSuccessors function call, and comment the foreach loop below)
             *
             */


            //GetSuccessors(current, startNode, destinationNode, ref open, ref closed);


            /*
             * 
             *  A* implementation
             * 
             */
            foreach (Node neighbour in m_grid.GetNeighbours(current))
            {
                if (!neighbour.isWalkable || closed.Contains(neighbour)) continue;

                int newMovementCost = current.gCost + m_grid.GetDistance(current, neighbour);

                if (newMovementCost < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCost;
                    neighbour.hCost = m_grid.GetDistance(neighbour, destinationNode);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour)) open.Enqueue(neighbour);
                }
            }
        }

        

    }


    void OnDrawGizmos()
    {
        if (!drawPath) return;

        if (fullPath == null && fullPath.Count < 1) return;

        Gizmos.color = Color.blue;

        foreach (Node n in fullPath)
        {
            float nodeDiameter = m_grid.nodeLength * 2;
            Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter - 0.1f, 1, nodeDiameter - 0.1f));

        }
    }



    void Trace(Node start, Node destination)
    {
        fullPath.Clear();
        Node current = destination;
        while(current != start)
        {
            fullPath.Add(current);
            current = current.parent;
        }
        fullPath.Reverse();
        pathPending = false;
    }

    //JPS+
    void GetSuccessors(Node current, Node start, Node end, ref Queue<Node> open, ref HashSet<Node> closed)
    {
        List<Node> neigbours = GetNeighbourJumppoints(current, end);

        foreach (Node neighbour in neigbours)
        {
            int dirX = Mathf.Clamp(neighbour.xGridPos - current.xGridPos, -1, 1);
            int dirY = Mathf.Clamp(neighbour.yGridPos - current.yGridPos, -1, 1);

            Node jumppoint = Jump(current.xGridPos, current.yGridPos, dirX, dirY, neighbour, end);

            if (jumppoint == null) continue;

            if (!jumppoint.isWalkable || closed.Contains(jumppoint)) continue;

            int newMovementCost = current.gCost + m_grid.GetDistance(current, jumppoint);

            if (newMovementCost < jumppoint.gCost || !open.Contains(jumppoint))
            {
                jumppoint.gCost = newMovementCost;
                jumppoint.hCost = m_grid.GetDistance(jumppoint, end);
                jumppoint.parent = current;
                if (!open.Contains(jumppoint)) open.Enqueue(jumppoint);
            }
        }
    }

    //JPS+
    List<Node> GetNeighbourJumppoints(Node current, Node end)
    {

        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0) continue;

                int neighborX = current.xGridPos + x;
                int neighborY = current.yGridPos + y;

                while(m_grid.grid[neighborX, neighborY].isWalkable)
                {

                    if(neighborX == end.xGridPos || neighborY == end.yGridPos)
                    {
                        neighbours.Add(m_grid.grid[neighborX, neighborY]);
                    }

                    neighborX += x;
                    neighborY += y;
                }

                if(m_grid.grid[neighborX - x, neighborY - y] != current)
                    neighbours.Add(m_grid.grid[neighborX - x, neighborY - y]);
                
            }
        }

        return neighbours;
    }

    struct JumpPoint
    {
        public int dirX;
        public int dirY;
        public int x;
        public int y;
    }

    //JPS+
    Node Jump(int x, int y, int dx, int dy, Node start, Node end)
    {
        int nextX = x + dx;
        int nextY = y + dy;

        if (!m_grid.grid[nextX, nextY].isWalkable) return null;

        if (nextX == end.xGridPos && nextY == end.yGridPos) return m_grid.grid[nextX, nextY];

        if (dx != 0 && dy != 0)
        {
            if ((m_grid.grid[x - dx, nextY].isWalkable && !m_grid.grid[x - dx, y].isWalkable) ||
               (m_grid.grid[nextX, y - dy].isWalkable && !m_grid.grid[x, y - dy].isWalkable))
                return m_grid.grid[nextX, nextY];

            if (Jump(nextX, nextY, dx, 0, start, end) != null ||
                Jump(nextX, nextY, 0, dy, start, end) != null)
            {
                return m_grid.grid[nextX, nextY];
            }
        }
        else
        {
            if (dx != 0)
            {
                if (m_grid.grid[nextX, nextY].isWalkable && !m_grid.grid[x, nextY].isWalkable ||
                   m_grid.grid[nextX, y - 1].isWalkable && !m_grid.grid[x, y - 1].isWalkable)
                {
                    return m_grid.grid[nextX, nextY];
                }
            }
            else
            {
                if (m_grid.grid[x + 1, nextY].isWalkable && !m_grid.grid[x + 1, y].isWalkable ||
                    m_grid.grid[x - 1, nextY].isWalkable && !m_grid.grid[x - 1, y].isWalkable)
                {
                    return m_grid.grid[nextX, nextY];
                }
            }
        }

        if (m_grid.grid[nextX, y].isWalkable && m_grid.grid[x, nextY].isWalkable)
            return Jump(nextX, nextY, dx, dy, start, end);
        else
            return null;

    }



}
