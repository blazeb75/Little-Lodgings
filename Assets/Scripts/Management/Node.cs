using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum State { Open, Occupied, Reserved}

    [HideInInspector] public GridBoard parent;
    public Furniture furniture;
    public List<Furniture> reservingFurniture;
    public new BoxCollider collider;

    public State state
    {
        get
        {
            if (furniture != null && reservingFurniture.Count != 0) Debug.LogError("Node is both occupied and reserved.", this);
            if (furniture != null) return State.Occupied;
            else if (reservingFurniture.Count != 0) return State.Reserved;
            else return State.Open;
        }
    }

    private void Awake()
    {
        parent = transform.parent.gameObject.GetComponent<GridBoard>();
        if (parent == null)
        {
            Debug.LogError("Node has no GridBoard in parent object", this);
        }

        collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        collider.size = new Vector3(parent.parent.interval, 0.25f, parent.parent.interval);
    }

    public void Occupy(Furniture furniture)
    {
        this.furniture = furniture;
    }
}
