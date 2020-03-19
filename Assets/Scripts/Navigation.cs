using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

    public IEnumerator SetDestination(Vector3 Destination)
    {
        
        m_destination = Destination;
        Node startNode = m_grid.NodeFromWorldPoint(transform.position);
        Node destinationNode = m_grid.NodeFromWorldPoint(Destination);
        int remainingGridDistance;

        Queue<Node> open = new Queue<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        open.Enqueue(startNode);

        pathPending = true;

        while(open.Count > 0)
        {
            Node current = open.Dequeue();

            closed.Add(current);

            remainingGridDistance = m_grid.GetDistance(current, destinationNode);

            if (remainingGridDistance == 0)
            {
                Trace(startNode, destinationNode);
                yield return null;
            }

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
            float nodeDiameter = m_grid.nodeRadius * 2;
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


    public void GetSuccessors(Node current, Node start, Node end, ref Queue<Node> open, ref HashSet<Node> closed)
    {
        List<Node> neigbours = GetNeighbourJumpoints(current, end);

        foreach (Node neighbour in neigbours)
        {
            int dirX = Mathf.Clamp(neighbour.xGridPos - current.xGridPos, -1, 1);
            int dirY = Mathf.Clamp(neighbour.yGridPos - current.yGridPos, -1, 1);

            Node jumppoint = jump(current.xGridPos, current.yGridPos, dirX, dirY, neighbour, end);

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

    List<Node> GetNeighbourJumpoints(Node current, Node end)
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

    struct JumpPoint //: IHeapItem<JumpPoint>
    {
        public int dirX;
        public int dirY;
        public int x;
        public int y;
        //int heapIndex;

        //int IHeapItem<JumpPoint>.HeapIndex
        //{
        //    get
        //    {
        //        return heapIndex;
        //    }
        //    set
        //    {
        //        heapIndex = value;
        //    }
        //}

        //int IComparable<JumpPoint>.CompareTo(JumpPoint other)
        //{
        //    if (x.CompareTo(other.x) < 0 && y.CompareTo(other.y) < 0)
        //        return -1;
        //    else if (x.CompareTo(other.x) > 0 && y.CompareTo(other.y) > 0)
        //        return 1;
        //    else
        //        return 0;
        //}
    }


    Node jump(int x, int y, int dx, int dy, Node start, Node end)
    {
        //List<JumpPoint> current = new List<JumpPoint>();

        //JumpPoint jmp = new JumpPoint
        //{
        //    dirX = dx,
        //    dirY = dy,
        //    x = x,
        //    y = y
        //};

        //int nextX;
        //int nextY;

        //current.Add(jmp);

        //while (current.Count > 0)
        //{
        //    jmp = current[0];

        //    nextX = jmp.x + jmp.dirX;
        //    nextY = jmp.y + jmp.dirY;

        //    current.RemoveAt(0);

        //    if (!m_grid.grid[nextX, nextY].isWalkable) continue;

        //    if (nextX == end.xGridPos && nextY == end.yGridPos) return m_grid.grid[nextX, nextY];

        //    if (jmp.dirX != 0 && jmp.dirY != 0)
        //    {
        //        if ((m_grid.grid[jmp.x - jmp.dirX, nextY].isWalkable && !m_grid.grid[jmp.x - jmp.dirX, jmp.y].isWalkable) ||
        //           (m_grid.grid[nextX, jmp.y - jmp.dirY].isWalkable && !m_grid.grid[jmp.x, jmp.y - jmp.dirY].isWalkable))
        //            return m_grid.grid[nextX, nextY];
        //        else
        //        {
        //            int tmpdy = jmp.dirY;
        //            int tmpdx = jmp.dirX;

        //            jmp.dirY = 0;
        //            if (!current.Contains(jmp))
        //            {
        //                jmp.x = nextX;
        //                jmp.y = nextY;
        //                current.Insert(0, jmp);
        //            }

        //            jmp.dirY = tmpdy;
        //            jmp.dirX = 0;
        //            if (!current.Contains(jmp))
        //            {
        //                jmp.x = nextX;
        //                jmp.y = nextY;
        //                current.Insert(0, jmp);
        //            }
        //            jmp.dirX = tmpdx;


        //        }

        //        //if (jump(nextX, nextY, dx, 0, start, end) != null ||
        //        //    jump(nextX, nextY, 0, dy, start, end) != null)
        //        //{
        //        //    return m_grid.grid[nextX, nextY];
        //        //}
        //        if (m_grid.grid[nextX, jmp.y].isWalkable && m_grid.grid[jmp.x, nextY].isWalkable)
        //        {
        //            current.Add(jmp);
        //        }

        //    }
        //    else
        //    {
        //        if (jmp.dirX != 0)
        //        {
        //            if (m_grid.grid[nextX, nextY].isWalkable && !m_grid.grid[jmp.x, nextY].isWalkable ||
        //               m_grid.grid[nextX, jmp.y - 1].isWalkable && !m_grid.grid[jmp.x, jmp.y - 1].isWalkable)
        //            {
        //                return m_grid.grid[nextX, nextY];
        //            }
        //        }
        //        else
        //        {
        //            if (m_grid.grid[jmp.x + 1, nextY].isWalkable && !m_grid.grid[jmp.x + 1, jmp.x].isWalkable ||
        //                m_grid.grid[jmp.x - 1, nextY].isWalkable && !m_grid.grid[jmp.x - 1, jmp.y].isWalkable)
        //            {
        //                return m_grid.grid[nextX, nextY];
        //            }
        //        }

        //        jmp.x = nextX;
        //        jmp.y = nextY;

        //        if (m_grid.grid[nextX, jmp.y].isWalkable && m_grid.grid[jmp.x, nextY].isWalkable)
        //        {
        //            current.Insert(0, jmp);
        //        }

        //    }



        //}

        //return null;

        int nextX = x + dx;
        int nextY = y + dy;

        if (!m_grid.grid[nextX, nextY].isWalkable) return null;

        if (nextX == end.xGridPos && nextY == end.yGridPos) return m_grid.grid[nextX, nextY];

        if (dx != 0 && dy != 0)
        {
            if ((m_grid.grid[x - dx, nextY].isWalkable && !m_grid.grid[x - dx, y].isWalkable) ||
               (m_grid.grid[nextX, y - dy].isWalkable && !m_grid.grid[x, y - dy].isWalkable))
                return m_grid.grid[nextX, nextY];

            if (jump(nextX, nextY, dx, 0, start, end) != null ||
                jump(nextX, nextY, 0, dy, start, end) != null)
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
            return jump(nextX, nextY, dx, dy, start, end);
        else
            return null;

    }



}
