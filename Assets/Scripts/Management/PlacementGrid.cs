using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.EventSystems;

public class PlacementGrid : MonoBehaviour
{
    public Camera editModeCamera;
    public GameObject nodePrefab;
    /// <summary>
    /// The size of each grid tile
    /// </summary>
    public float interval = 2;
    public Vector2 size;
    public Vector2 origin;
    public List<Node> nodes;
    public List<Furniture> furniture;

    [Header("Debug")]
    public Node[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();

    }

    public void CreateGrid()
    {
        grid = new Node[(int)size.x, (int)size.y];

        nodes.Clear();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector3 pos = transform.position;
                pos.x += origin.x;
                pos.z += origin.y;
                pos += transform.right * i * interval;
                pos += transform.forward * j * interval;

                GameObject nodeObj = Instantiate(nodePrefab, pos, transform.rotation, transform);
                if (nodeObj != null)
                {
                    Node node = nodeObj.GetComponent<Node>();
                    nodes.Add(node);
                    grid[i, j] = node;
                    node.position = new Vector2(i, j);
                    nodeObj.name = i + ", " + j;
                    node.gameObject.SetActive(false);
                }

            }

        }

    }

    public bool Clear()
    {
        if (furniture.Count == 0)
        {
            return false;
        }
        else
        {
            Furniture[] furns = furniture.ToArray();
            foreach (Furniture furn in furns)
            {
                Destroy(furn.gameObject);
            }
            Debug.Log("Cleared");
            return true;
        }
    }

    public Node CursorNode()
    {
        Ray ray = GameManager.activeCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Grid"), QueryTriggerInteraction.Collide);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.parent == transform)
            {
                return hit.collider.GetComponent<Node>();
            }
        }
        return null;
    }

    public Furniture CursorFurniture(out bool hitUI)
    {
        hitUI = false; 
        if (EventSystem.current.IsPointerOverGameObject(-1))
        {
            hitUI = true;
            //Debug.Log("Clicked event object");
            return null;
        }

        Ray ray = GameManager.activeCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Furniture"), QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            
            if (hit.transform.parent == transform)
            {
                return hit.transform.GetComponent<Furniture>();
            }
        }
        return null;
    }

    public void SetGridActive(bool state)
    {
        foreach (Node node in nodes)
        {
            node.gameObject.SetActive(state);
        }
    }

    public Node GetNode(int x, int y)
    {
        try
        {
            return grid[x, y];
        }
        catch (System.IndexOutOfRangeException)
        {            
            return null;
        }
    }
    public Node GetNode(float x, float y)
    {
        return GetNode((int)x, (int)y);
    }
    public Node GetNode(Vector2 position)
    {
        return GetNode((int)position.x, (int)position.y);
    }
    public Vector3 GetCenter()
    {
        Vector3 middle = transform.position;
        middle.x += size.x / 2f;
        middle.z += size.y / 2f;
        return middle;
    }
    public void EnterEditMode()
    {
        Decorator.instance.targetGrid = this;
        GameManager.instance.SwitchCamera(editModeCamera);
        Decorator.instance.editModeCanvas.SetActive(true);
        Decorator.instance.OnEnterEditMode.Invoke();
    }
    public void ExitEditMode()
    {
        Decorator.instance.SelectedObject = null;
        Decorator.instance.targetGrid = null;
        GameManager.instance.SwitchCamera(GameManager.instance.freeCamera);
        Decorator.instance.editModeCanvas.SetActive(false);
        Decorator.instance.OnExitEditMode.Invoke();
    }

    private void OnEnable()
    {
        foreach (Node node in nodes)
        {
            node.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (Node node in nodes)
        {
            node.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
        {
            Vector3 nodeSize = new Vector3(interval, 0.2f, interval);
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Vector3 pos = transform.position;
                    pos.x += origin.x;
                    pos.z += origin.y;
                    pos += transform.right * i * interval;
                    pos += transform.forward * j * interval;
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(pos, nodeSize);
                }
            }
        }
        else
        {
            foreach (Node node in nodes)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(node.transform.position, node.collider.size);
            }
        }
    }
#endif
}
