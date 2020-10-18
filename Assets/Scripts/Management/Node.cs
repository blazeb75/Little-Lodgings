using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
    public enum State { Open, Occupied, Reserved, Unused}

    [HideInInspector] public PlacementGrid parent;
    private Furniture furniture;
    public List<Furniture> reservingFurniture;
    public new BoxCollider collider;
    public Vector2 position;

    private NavMeshObstacle nmo;

    [SerializeField] State state;

    public Furniture Furniture
    {
        get => furniture;
        set
        {
            furniture = value;
            if(furniture == null)
            {
                nmo.enabled = false;
            }
            else
            {
                nmo.enabled = true;
            }
            //if (value == null)
            //    GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public State GetState()
    {
        if (state == State.Unused)
            return state;
        if (Furniture != null && reservingFurniture.Count != 0)
            Debug.LogError("Node is both occupied and reserved.", this);
        if (Furniture != null)
            state = State.Occupied;
        else if (reservingFurniture.Count != 0)
            state = State.Reserved;
        else
            state = State.Open;
        return state;
    }

    private void Awake()
    {
        if (GetState() == State.Unused)
            Destroy(gameObject);

        parent = transform.parent.gameObject.GetComponent<PlacementGrid>();
        if (parent == null)
        {
            Debug.LogError("Node has no GridBoard in parent object", this);
        }

        collider = GetComponent<BoxCollider>();
        nmo = GetComponent<NavMeshObstacle>();
    }

    private void Start()
    {
        collider.size = new Vector3(parent.interval, 0.25f, parent.interval);
    }

    public void Occupy(Furniture furniture)
    {
        this.Furniture = furniture;
        //GetComponent<Renderer>().material.color = Color.red;
    }
}
