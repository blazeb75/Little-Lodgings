using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementGrid : MonoBehaviour
{

    /// <summary>
    /// The size of each grid tile
    /// </summary>
    public float interval = 2;

    public List<Node> nodes;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    public GridBoard[] GridBoards()
    {
        return (GetComponentsInChildren<GridBoard>());
    }

    public void CreateGrid()
    {
        nodes.Clear();
        foreach(GridBoard gb in GridBoards())
        {
            nodes.AddRange(gb.nodes);
        }
    }

    public Node CursorNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Collide);
        if (hit.collider == null)
            return null;
        else
            return hit.collider.GetComponent<Node>();
    }

    public Furniture CursorFurniture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Furniture"), QueryTriggerInteraction.Ignore);
        if (hit.collider != null && hit.collider.transform.parent.parent == transform)
            return hit.collider.GetComponent<Furniture>();
        else return null;
    }

    public void SetGridActive(bool state)
    {
        foreach(Node node in nodes)
        {
            node.gameObject.SetActive(state);
        }
    }
}
