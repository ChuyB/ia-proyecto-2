using System.Collections.Generic;
using UnityEngine;

class Graph
{
    private List<Node> nodes = new List<Node>();

    public List<Node> GetNodes()
    {
        return nodes;
    }

    public Connection[] GetConnections(Node fromNode)
    {
        return fromNode.connections.ToArray();
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public void AddConnection(Node fromNode, Node toNode, float cost)
    {
        fromNode.connections.Add(new Connection(fromNode, toNode, cost));
    }
}

class Connection
{
    public Node fromNode;
    public Node toNode;
    private float cost;

    public Connection(Node from, Node to, float cost)
    {
        fromNode = from;
        toNode = to;
        this.cost = cost;
    }

    public float GetCost()
    {
        return cost;
    }
}

class Node
{
    public Vector3 position;
    public List<Connection> connections = new List<Connection>();

    public Node(Vector3 pos)
    {
        position = pos;
    }
}
