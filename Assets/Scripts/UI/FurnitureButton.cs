using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureButton : MonoBehaviour
{
    private bool placeInstantly = true;
    private Decorator decorator;
    public GameObject furniture;
    Button button;

    private void Start()
    {
        if(!TryGetComponent(out button))
        {
            Debug.LogError("Furniture button has no Button component.", this);
        }
        else
        {
            button.onClick.AddListener(MessageDecorator);
            decorator = Decorator.instance;
        }
    }

    void MessageDecorator()
    {
        if (placeInstantly)
        {
            if (!decorator.TryCreateNearCenter(furniture))
            {
                Debug.LogWarning("Could not place " + furniture.name);
            }
        }
        else
        {
            decorator.CreatePlacingObject(furniture);
        }
        Decorator.instance.catalogueCanvas.SetActive(false);
        Decorator.instance.editModeCanvas.SetActive(true);
    }
}
