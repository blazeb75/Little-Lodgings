using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Decorator : MonoBehaviour
{
    public static Decorator instance;

    public UnityEvent OnFurnitureSelected;
    public UnityEvent OnFurnitureDeselected;

    public PlacementGrid targetGrid;
    private GameObject selectedObject;
    public GameObject selectedPrefab;
    public int selectedRotation;

    public enum Mode { Idle, PlacingObject }
    public Mode mode;

    private Node previousNode;

    public GameObject SelectedObject
    {
        get => selectedObject;
        set
        {
            selectedObject = value;
            if(selectedObject == null)
            {
                OnFurnitureDeselected.Invoke();
            }
            else
            {
                OnFurnitureSelected.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Duplicate Decorator", this);
        }
        else
        {
            instance = this;
        }
    }

    public bool ChangeMode(Mode newMode)
    {
        if (newMode == mode)
        {
            return false;
        }
        else
        {
            if (mode == Mode.PlacingObject)
            {
                CancelPlaceObject();
            }
            if (newMode == Mode.PlacingObject)
            {
                StartPlacingObject();
            }

            mode = newMode;
            return true;
        }
    }

    private void Update()
    {
        if (mode == Mode.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Furniture hit = targetGrid.CursorFurniture();
                if (hit != null)
                {
                    SelectedObject = hit.gameObject;
                    hit.Remove();
                    ChangeMode(Mode.PlacingObject);
                }
            }
        }

        else if (mode == Mode.PlacingObject)
        {
            PlacingObject();
        }
    }

    void StartPlacingObject()
    {
        mode = Mode.PlacingObject;
        targetGrid.SetGridActive(true);
        if (SelectedObject == null)
        {
            SelectedObject = Instantiate(selectedPrefab);
        }
        previousNode = SelectedObject.GetComponent<Furniture>().node;
    }

    void PlacingObject()
    {
        Node node = targetGrid.CursorNode();
        Furniture furniture = SelectedObject.GetComponent<Furniture>();
        furniture.node = node;

        if (node == null || furniture.GetOverlappingNodes().Contains(null))
        {
            SelectedObject.SetActive(false);
            return;
        }
        else
        {
            SelectedObject.SetActive(true);
        }

        furniture.SnapToNode(node);
        SelectedObject.transform.rotation = node.transform.rotation;

        if (Controls.Rotate())
        {
            selectedRotation += 1;
            if (selectedRotation == 4)
                selectedRotation = 0;
        }

        SelectedObject.transform.Rotate(0, 90 * selectedRotation, 0);


#if UNITY_IOS || UNITY_ANDROID
        if(Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (furniture.CanPlaceHere())
            {
                PlaceObject(node);
            }
            else
            {
                PlaceObject(previousNode);
            }
        }
#else
        if (Input.GetMouseButtonUp(0) && furniture.CanPlaceHere())
        {
            if (furniture.CanPlaceHere())
            {
                PlaceObject(node);
            }
            else
            {
                PlaceObject(previousNode);
            }
        }
#endif
    }

    void PlaceObject(Node node)
    {
        Furniture furniture = SelectedObject.GetComponent<Furniture>();
        furniture.Place(node);
        furniture.node = null;
        SelectedObject = null;
        previousNode = null;
        selectedRotation = 0;
        ChangeMode(Mode.Idle);
    }

    public void CancelPlaceObject()
    {
        mode = Mode.Idle;
        targetGrid.SetGridActive(false);
        if (SelectedObject != null)
        {
            Destroy(SelectedObject);
            SelectedObject = null;
        }
        selectedRotation = 0;
    }

    public void Rotate()
    {
        selectedRotation += 1;
        if (selectedRotation == 4) selectedRotation = 0;
    }

    public bool CreatePlacingObject(GameObject obj)
    {
        if (mode != Mode.Idle)
        {
            return false;
        }
        SelectedObject = CreateFurniture(obj, targetGrid);
        ChangeMode(Mode.PlacingObject);
        return true;
    }

    public void Clear()
    {
        if(mode == Mode.PlacingObject)
        {
            CancelPlaceObject();
        }
        targetGrid.Clear();
    }

    public static GameObject CreateFurniture(GameObject prefab, PlacementGrid grid)
    {
        Debug.Log("Instantiated " + prefab.name);
        GameObject go = Instantiate(prefab);
        go.transform.parent = grid.transform;
        Furniture furn = go.GetComponent<Furniture>();
        furn.grid = grid;
        grid.furniture.Add(furn);
        return go;
    }
}
