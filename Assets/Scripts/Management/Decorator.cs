using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : MonoBehaviour
{
    public PlacementGrid targetGrid;
    public GameObject selectedObject;
    public GameObject selectedPrefab;
    public int selectedRotation;

    public enum Mode { Idle, PlacingObject }
    public Mode mode;

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
                    selectedObject = hit.gameObject;
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
        targetGrid.SetGridActive(true);
        if (selectedObject == null)
        {
            selectedObject = Instantiate(selectedPrefab);
        }
    }

    void PlacingObject()
    {
        Node node = targetGrid.CursorNode();
        Furniture furniture = selectedObject.GetComponent<Furniture>();
        furniture.node = node;

        if (node == null || furniture.GetOverlappingNodes().Contains(null))
        {
            selectedObject.SetActive(false);
            return;
        }
        else
        {
            selectedObject.SetActive(true);
        }

        furniture.SnapToNode(node);
        selectedObject.transform.rotation = node.transform.rotation;

        if (Controls.Rotate())
        {
            selectedRotation += 1;
            if (selectedRotation == 4)
                selectedRotation = 0;
        }

        selectedObject.transform.Rotate(0, 90 * selectedRotation, 0);

        if ((Input.GetMouseButtonDown(0) || Controls.Accept()) && furniture.CanPlaceHere())
        {
            PlaceObject(node);
        }
    }

    void PlaceObject(Node node)
    {
        Furniture furniture = selectedObject.GetComponent<Furniture>();
        furniture.Place(node);
        furniture.node = null;
        selectedObject = null;
        selectedRotation = 0;
        ChangeMode(Mode.Idle);
    }

    void CancelPlaceObject()
    {
        targetGrid.SetGridActive(false);
        if (selectedObject != null)
        {
            Destroy(selectedObject);
        }
        selectedRotation = 0;
    }
    public bool CreatePlacingObject(GameObject obj)
    {
        if (mode != Mode.Idle)
        {
            return false;
        }

        selectedObject = Instantiate(obj);
        selectedObject.transform.parent = targetGrid.transform;
        selectedObject.GetComponent<Furniture>().grid = targetGrid;
        ChangeMode(Mode.PlacingObject);
        return true;
    }
}
