using System.Collections.Generic;
using UnityEngine;

public class AStarGraph : MonoBehaviour
{
    [Header("Point Generation")]
    public int numberOfPoints = 10;
    public Vector3 rangeMin = new Vector3(-10, -10, -10);
    public Vector3 rangeMax = new Vector3(10, 10, 10);
    public GameObject spherePrefab;

    [Header("Graph Settings")]
    public int k = 3;
    public Material edgeMaterial;
    public Material pathMaterial;

    private List<Node> nodes = new List<Node>();
    private Node startNode;
    private Node endNode;

    public class Node
    {
        public Vector3 position;
        public Dictionary<Node, float> neighbors = new Dictionary<Node, float>();

        // A* bookkeeping
        public Node parent;
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;

        public Node(Vector3 pos) { position = pos; }
    }

    void Start()
    {
        GenerateNodes();
        FindKNearestNeighbors();

        startNode = nodes[0];
        endNode = nodes[nodes.Count - 1];

        VisualizeGraph();

        List<Node> path = AStar(startNode, endNode);
        if (path != null)
        {
            DrawPath(path);
            PrintPathInfo(path);
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    void GenerateNodes()
    {
        nodes.Clear();
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(rangeMin.x, rangeMax.x),
                Random.Range(rangeMin.y, rangeMax.y),
                Random.Range(rangeMin.z, rangeMax.z)
            );

            Node node = new Node(pos);
            nodes.Add(node);

            if (spherePrefab != null)
                Instantiate(spherePrefab, pos, Quaternion.identity);
            else
                GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = pos;
        }
    }

    void FindKNearestNeighbors()
    {
        foreach (Node node in nodes)
        {
            List<(Node, float)> distances = new List<(Node, float)>();

            foreach (Node other in nodes)
            {
                if (other == node) continue;
                float dist = Vector3.Distance(node.position, other.position);
                distances.Add((other, dist));
            }

            distances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            for (int i = 0; i < Mathf.Min(k, distances.Count); i++)
            {
                Node neighbor = distances[i].Item1;
                float dist = distances[i].Item2;
                if (!node.neighbors.ContainsKey(neighbor))
                    node.neighbors.Add(neighbor, dist);
                if (!neighbor.neighbors.ContainsKey(node)) // keep graph undirected
                    neighbor.neighbors.Add(node, dist);
            }
        }
    }

    List<Node> AStar(Node start, Node goal)
    {
        List<Node> openList = new List<Node> { start };
        HashSet<Node> closedList = new HashSet<Node>();

        start.gCost = 0;
        start.hCost = Vector3.Distance(start.position, goal.position);
        start.parent = null;

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.fCost.CompareTo(b.fCost));
            Node current = openList[0];

            if (current == goal)
                return ReconstructPath(goal);

            openList.Remove(current);
            closedList.Add(current);

            foreach (var kvp in current.neighbors)
            {
                Node neighbor = kvp.Key;
                float cost = kvp.Value;

                if (closedList.Contains(neighbor)) continue;

                float tentativeG = current.gCost + cost;
                bool isBetter = false;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                    isBetter = true;
                }
                else if (tentativeG < neighbor.gCost)
                {
                    isBetter = true;
                }

                if (isBetter)
                {
                    neighbor.parent = current;
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Vector3.Distance(neighbor.position, goal.position);
                }
            }
        }

        return null; // no path
    }

    List<Node> ReconstructPath(Node goal)
    {
        List<Node> path = new List<Node>();
        Node current = goal;
        while (current != null)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    void VisualizeGraph()
    {
        foreach (Node node in nodes)
        {
            foreach (var kvp in node.neighbors)
            {
                Node neighbor = kvp.Key;
                DrawLine(node.position, neighbor.position, edgeMaterial);
            }
        }
    }

    void DrawPath(List<Node> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            DrawLine(path[i].position, path[i + 1].position, pathMaterial);
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Material mat)
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = mat;
        lr.startWidth = 0.07f;
        lr.endWidth = 0.07f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void PrintPathInfo(List<Node> path)
    {
        float totalLength = 0f;
        for (int i = 0; i < path.Count - 1; i++)
            totalLength += Vector3.Distance(path[i].position, path[i + 1].position);

        Debug.Log($"A* Path found! Length={totalLength:F2}, Nodes={path.Count}");
    }
}
