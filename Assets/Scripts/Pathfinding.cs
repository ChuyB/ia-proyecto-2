using UnityEngine;
using System.Collections.Generic;

class Pathfinding
{
    private Graph graph;

    public Pathfinding(Graph graph) { this.graph = graph; }

    public List<Node> FindPath(Node startNode, Node goalNode)
    {
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (Node node in graph.GetNodes())
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, goalNode);

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            foreach (Node node in openSet)
            {
                if (fScore[node] < fScore[current])
                    current = node;
            }

            if (current == goalNode)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Connection connection in graph.GetConnections(current))
            {
                Node neighbor = connection.toNode;

                if (closedSet.Contains(neighbor))
                    continue;

                float tentative_gScore = gScore[current] + connection.GetCost();

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);

                if (tentative_gScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goalNode);
            }
        }

        Debug.Log("Path not found");
        return null; // Path not found
    }

    private float Heuristic(Node a, Node b)
    {
        // Use Euclidean distance as heuristic
        return Vector3.Distance(a.position, b.position);
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom,
                                       Node current)
    {
        List<Node> totalPath = new List<Node> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse();
        return totalPath;
    }
}
