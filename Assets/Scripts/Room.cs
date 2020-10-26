using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlacementGrid))]
public class Room : MonoBehaviour
{
    public int roomID;
    public bool isBedroom;
    [HideInInspector] public PlacementGrid grid;

    private void Start()
    {
        if (!grid)
        {
            grid = GetComponent<PlacementGrid>();
        }
        if (isBedroom)
            GuestManager.instance.rooms.Add(this);
    }
    public Node RandomNodeInRoom()
    {
        Node[] nodes = grid.nodes.Where(x => x.GetState() != Node.State.Occupied).ToArray();
        int index = Random.Range(0, nodes.Length);
        return nodes[index];
    }
}
