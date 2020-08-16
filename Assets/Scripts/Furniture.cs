using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public Vector3 origin;
    public PlacementGrid grid;
    public Vector2[] reservations;

    public List<Node> occupiedNodes;
    public List<Node> reservedNodes;

    private void Start()
    {
        if (grid == null)
            grid = transform.parent.parent.gameObject.GetComponent<PlacementGrid>();
    }

    List<Node> GetOverlappingNodes()
    {        
        Collider col = GetComponent<Collider>();
        List<Node> nodes = new List<Node>();
        foreach (Node node in grid.GetComponentsInChildren<Node>())
        {
            if (col.bounds.Intersects(node.collider.bounds))
            {
                nodes.Add(node);
            }
        }

        return nodes;
    }

    List<Node> GetReserveNodes(Node originNode)
    {
        List<Node> nodes = new List<Node>();
        foreach (Vector2 res in reservations)
        {
            Vector3 nodePos = transform.position - origin;
            float interval = originNode.parent.parent.interval;
            nodePos.x += res.x * interval;
            nodePos.z += res.y * interval;
            Collider[] overlaps = Physics.OverlapBox(nodePos, Vector3.one, transform.rotation, LayerMask.GetMask("Grid"));
            if(overlaps.Length > 1)
            {
                Debug.LogWarning("Multiple nodes hit. " + overlaps.Length + " " + overlaps);
            }
            if (overlaps.Length == 1)
            {
                Node resNode = overlaps[0].GetComponent<Node>();
                nodes.Add(resNode);
            }
        }
        return nodes;
    }

    public bool CanPlace(Node node)
    {
        transform.position = node.transform.position + origin;
        foreach (Node n in GetOverlappingNodes())
        {
            if (n.state != Node.State.Open)
            {
                return false;
            }
        }
        foreach (Node n in GetReserveNodes(node))
        {
            if (n.state == Node.State.Occupied)
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
        foreach(Node n in GetReserveNodes(node))
        {
            n.reservingFurniture.Add(this);
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
