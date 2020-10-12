using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Decorator : MonoBehaviour
{
    public static Decorator instance;

    public UnityEvent OnFurnitureSelected;
    public UnityEvent OnNoFurnitureSelected;

    public Material selectionMaterial;
    public PlacementGrid targetGrid;
    private GameObject selectedObject;
    public GameObject selectedPrefab;
    public int selectedRotation;

    public enum Mode { Idle, PlacingObject }
    public Mode mode;

    private Node previousNode;

    public GameObject editModeCanvas;

    public GameObject SelectedObject
    {
        get => selectedObject;
        set
        {
            if(selectedObject != null)
            {
                Renderer r = selectedObject.GetComponentInChildren<Renderer>();
                //r.material.color = Color.white;
                //r.material.shader = Shader.Find("Standard");
                Destroy(r.gameObject.GetComponent<Outline>());
            }

            selectedObject = value;
            if(selectedObject == null)
            {
                OnNoFurnitureSelected.Invoke();
            }
            else
            {
                OnFurnitureSelected.Invoke(); 
                
                Renderer r = selectedObject.GetComponentInChildren<Renderer>();
                //r.materials = r.materials.Append(selectionMaterial).ToArray();
                //r.material.color = Color.yellow;
                //r.material.shader = Shader.Find("Outlined/UltimateOutlineShadows");
                r.gameObject.AddComponent<Outline>();
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

    private void Start()
    {
        editModeCanvas.SetActive(false);
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
        if(targetGrid == null)
        {
            return;
        }
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
    public void ExitEditMode()
    {
        targetGrid.ExitEditMode();
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

    public bool TryCreateNearCenter(GameObject obj)
    {
        return TryCreateNearCenter(obj, targetGrid, out _, out _);
    }
    public bool TryCreateNearCenter(GameObject obj, out GameObject newInstance, out Furniture newFurniture)
    {
        return TryCreateNearCenter(obj, targetGrid, out newInstance, out newFurniture);
    }
    public bool TryCreateNearCenter(GameObject obj, PlacementGrid grid, out GameObject newInstance, out Furniture newFurniture)
    {
        //if (mode != Mode.Idle)
        //{
        //    newInstance = null;
        //    return false;
        //}
        newInstance = CreateFurniture(obj, targetGrid);
        newFurniture = newInstance.GetComponent<Furniture>();

        int xs = Mathf.FloorToInt(grid.size.x / 2f), ys = Mathf.FloorToInt(grid.size.y / 2f); // Start coordinates

        // Check point (xs, ys)
        int d = 1, x1, x2, y1, y2;
        Node node;
        bool outOfRange = false;
        while (!outOfRange)
        {
            outOfRange = true;
            for (int i = 0; i < d + 1; i++)
            {
                x1 = xs - d + i;
                y1 = ys - i;

                // Check point (x1, y1)
                if (!(x1 >= grid.size.x || x1 < 0 || y1 >= grid.size.y || y1 < 0))
                {
                    outOfRange = false;
                    node = grid.GetNode(x1, y1);
                    if (newFurniture.CanPlaceHere(node))
                    {
                        newFurniture.Place(node);
                        return true;
                    }
                }

                x2 = xs + d - i;
                y2 = ys + i;

                // Check point (x2, y2)
                if (!(x2 >= grid.size.x || x2 < 0 || y2 >= grid.size.y || y2 < 0))
                {
                    outOfRange = false;
                    node = grid.GetNode(x2, y2);
                    if (newFurniture.CanPlaceHere(node))
                    {
                        newFurniture.Place(node);
                        return true;
                    }
                }
            }


            for (int i = 1; i < d; i++)
            {
                x1 = xs - i;
                y1 = ys + d - i;

                // Check point (x1, y1)
                if (!(x1 >= grid.size.x || x1 < 0 || y1 >= grid.size.y || y1 < 0))
                {
                    outOfRange = false;
                    node = grid.GetNode(x1, y1);
                    if (newFurniture.CanPlaceHere(node))
                    {
                        newFurniture.Place(node);
                        return true;
                    }
                }

                x2 = xs + i;
                y2 = ys - d + i;

                // Check point (x2, y2)
                if (!(x2 >= grid.size.x || x2 < 0 || y2 >= grid.size.y || y2 < 0))
                {
                    outOfRange = false;
                    node = grid.GetNode(x2, y2);
                    if (newFurniture.CanPlaceHere(node))
                    {
                        newFurniture.Place(node);
                        return true;
                    }
                }
            }
            d++;
        }
        Destroy(newInstance);
        return false;
    }
}
