using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 *  Implementation based on Sebastian Lague
 *  'https://github.com/SebLague/Pathfinding'
 *
 */

//Class used for creating a grid of nodes used for implemented pathfinding algorithms
public class Grid : MonoBehaviour {

    public Vector2 gridWorldSize;
    public float nodeLength;
    public bool drawGrid;
    public Node[,] grid;

    bool m_isFirst = false;

    float m_nodeDiameter;
    int m_gridSizeX, m_gridSizeY;

   
	// Use this for initialization
	void Start () {
        m_nodeDiameter = nodeLength * 2;
        m_gridSizeX = Mathf.RoundToInt(gridWorldSize.x / m_nodeDiameter);
        m_gridSizeY = Mathf.RoundToInt(gridWorldSize.y / m_nodeDiameter);
        
  	}

    public int GridSizeX { get { return m_gridSizeX; } }
    public int GridSizeY { get { return m_gridSizeY; } }
    public int MaxSize { get { return m_gridSizeX * m_gridSizeY; } }

    IEnumerator CreateGrid()
    {
        grid = new Node[m_gridSizeX, m_gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < m_gridSizeX; ++x)
        {
            for(int y = 0; y < m_gridSizeY; ++y)
            {
                Vector3 pointInWorld = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + nodeLength) + Vector3.forward * (y * m_nodeDiameter + nodeLength);

                bool walkable = (Physics.CheckSphere(pointInWorld, nodeLength));

                grid[x, y] = new Node(walkable, pointInWorld, x, y);
            }
        }
        yield return null;
    }

    public List<Node> GetNeighbours(Node n)
    {

        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if (x == 0  && y == 0) continue;

                int neighborX = n.xGridPos + x;
                int neighborY = n.yGridPos + y;

                if (neighborX >= 0 && neighborX < m_gridSizeX && neighborY >= 0 && neighborY < m_gridSizeY)
                {
                    neighbours.Add(grid[neighborX, neighborY]);
                }
            }
        }

        return neighbours;

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid == null) return;

        if (!drawGrid) return;

        foreach (Node n in grid)
        {
            Gizmos.color = n.isWalkable ? Color.white : Color.red;

            Gizmos.DrawCube(n.worldPosition, new Vector3(m_nodeDiameter - 0.1f, 1, m_nodeDiameter - 0.1f));
        }
    }

    public int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(b.xGridPos - a.xGridPos);
        int distY = Mathf.Abs(b.yGridPos - a.yGridPos);

        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    public Node NodeFromWorldPoint(Vector3 worldPosistion)
    {
        float percentX = Mathf.Clamp01((worldPosistion.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosistion.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);

        return grid[x, y];
    }

	// Update is called once per frame
	void Update () {
		if(!m_isFirst)
        {
            StartCoroutine(CreateGrid());
            m_isFirst = true;
        }
	}
}
