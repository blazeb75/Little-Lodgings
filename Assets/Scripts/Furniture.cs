using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public enum Tags { Light, Table, Bed, Seat, Entertaining, Storage}

    [Header("Settings")]
    public Tags[] tags;
    public Vector2 size;
    public Vector2[] reservations;
    public Vector3 offset;

    [Header("Debug")]
    public PlacementGrid grid;
    public Node node;
    public List<Node> occupiedNodes;
    public List<Node> reservedNodes;

    public Vector3 origin
    {
        get => new Vector3(
            1 - size.x + offset.x,
            offset.y,
            1 - size.y + offset.z);
    }

    private void Start()
    {
        if (grid == null)
            grid = transform.parent.gameObject.GetComponent<PlacementGrid>();

        if(TryGetComponent<MeshCollider>(out MeshCollider col))
        {
            col.sharedMesh = GetComponentInChildren<MeshFilter>().mesh;
        }
    }

    private void OnDestroy()
    {
        if(grid != null)
        {
            grid.furniture.Remove(this);
        }
    }

    public List<Node> GetOverlappingNodes()
    {
        List<Node> nodes = new List<Node>();
        if(node == null)
        {
            return nodes;
        }
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                nodes.Add(grid.GetNode(node.position.x + i, node.position.y + j));
            }
        }
        return nodes;
    }

    public List<Node> GetReserveNodes()
    {
        List<Node> nodes = new List<Node>();
        foreach (Vector2 res in reservations)
        {
            nodes.Add(grid.GetNode(node.position + res));
        }
        return nodes;
    }

    public bool CanPlaceHere()
    {
        foreach (Node n in GetOverlappingNodes())
        {
            if (n == null || n.GetState() != Node.State.Open)
            {
                return false;
            }
        }
        foreach (Node n in GetReserveNodes())
        {
            if (n == null || !(n.GetState() == Node.State.Open || n.GetState() == Node.State.Reserved))
            {
                return false;
            }
        }
        return true;
    }

    public void SnapToNode(Node node)
    {
        transform.position = node.transform.position;
        transform.position -= origin;
    }

    public void Place(Node node)
    {
        SnapToNode(node);

        foreach(Node n in GetOverlappingNodes())
        {
            n.Occupy(this);
            occupiedNodes.Add(n);
        }
        foreach(Node n in GetReserveNodes())
        {
            n.reservingFurniture.Add(this);
            reservedNodes.Add(n);
        }
    }

    public void Remove()
    {
        foreach(Node n in occupiedNodes)
        {
            n.furniture = null;
        }
        occupiedNodes.Clear();
        foreach(Node n in reservedNodes)
        {
            n.reservingFurniture.Remove(this);
        }
        reservedNodes.Clear();
    }
}
