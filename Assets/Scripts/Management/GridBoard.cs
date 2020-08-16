using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    public GameObject nodePrefab;
    public Vector2 size;

    [HideInInspector] public PlacementGrid parent;
    [HideInInspector] public List<Node> nodes;

    private void Awake()
    {
        parent = transform.parent.gameObject.GetComponent<PlacementGrid>();
        if (parent == null)
        {
            Debug.LogError("GridBoard has no PlacementGrid in parent object", this);
        }
    }

    private void Start()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector3 pos = transform.position;
                pos += transform.forward * i * parent.interval;
                pos += transform.right * j * parent.interval;

                GameObject node = Instantiate(nodePrefab, pos, transform.rotation, transform);
                nodes.Add(node.GetComponent<Node>());
            }
        }
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
        foreach(Node node in nodes)
        {
            node.gameObject.SetActive(false);
        }
    }
}
