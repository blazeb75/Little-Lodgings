using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public enum Tags { 
        ERROR_INVALID,
        //Kinds of furniture
        Light, Table, Bed, Seat, Entertaining, Storage,
        //Aesthetic
        Base, Royal, Pirate, Enchanted, Occult,
        //Type
        Comfort, Surface, Renovation, Accessory,
        //Placement restrictions
        BedroomOnly
    }
    [Header("Settings")]
    public Tags[] tags;
    public Vector2 size = new Vector2 (1, 1);
    public Vector2[] reservations;
    public Vector3 offset;
    public float cost;
    public float unlockCost;

    [Header("Debug")]
    public PlacementGrid grid;
    public Node node;
    public List<Node> occupiedNodes;
    public List<Node> reservedNodes;

    void OnValidate()
    {
        if(size == new Vector2(0, 0))
        {
            size = new Vector2(1, 1);
        }
    }
    public Vector3 origin
    {
        get => new Vector3(

            (size.x % 2) / 2f - 0.5f + offset.x,
            offset.y,
            (size.y % 2) / 2f - 0.5f + offset.z);
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
        return GetOverlappingNodes(node);
    }
    public List<Node> GetOverlappingNodes(Node node)
    {
        float sizex, sizey;
        //if (true)//transform.rotation.y % 180 == 0)
        //{
        sizex = size.x;
        sizey = size.y;
        //}
        //else
        //{
        //    sizex = size.y;
        //    sizey = size.x;
        //}
        List<Node> nodes = new List<Node>();
        if(node == null)
        {
            return nodes;
        }
        for (int i = 1; i <= sizex; i++)
        {
            float x, y; 
            if (i % 2 == 1)
            {
                x = node.position.x + (-i / 2f) + 0.5f;
            }
            else
            {
                x = node.position.x + i / 2f;
            }
            for (int j = 1; j <= sizey; j++)
            {
                
                if (j % 2 == 1)
                {
                    y = node.position.y + (-j / 2f) + 0.5f;
                }
                else
                {
                    y = node.position.y + j / 2f;
                }
                //Debug.Log("In " + i.ToString() + j.ToString() + " Out " + x.ToString() + y.ToString());
                nodes.Add(grid.GetNode(x, y));
            }
        }
        return nodes;
    }

    public List<Node> GetReserveNodes()
    {
        return GetReserveNodes(node);
    }
    public List<Node> GetReserveNodes(Node node)
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
        return CanPlaceHere(node);
    }
    public bool CanPlaceHere(Node node)
    {
        if (node == null)         
        {
            Debug.LogWarning("Checked CanPlaceHere on null node");
            return false; 
        }
        foreach (Node n in GetOverlappingNodes(node))
        {
            if (n == null || !(n.Furniture == null || n.Furniture == this))
            {
                return false;
            }
        }
        foreach (Node n in GetReserveNodes(node))
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
        this.node = node;
    }

    public void Place(Node node, bool calledByPlayer = false)
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

        if (calledByPlayer)
        {
            Decorator.instance.SelectedObject = this.gameObject;
        }
    }

    public void Remove()
    {
        foreach(Node n in occupiedNodes)
        {
            n.Furniture = null;
        }
        occupiedNodes.Clear();
        foreach(Node n in reservedNodes)
        {
            n.reservingFurniture.Remove(this);
        }
        reservedNodes.Clear();
    }

    public void FlipDimensions()
    {
        size = new Vector2(size.y, size.x);
        offset = new Vector3(offset.z, offset.y, offset.x);
    }
}
