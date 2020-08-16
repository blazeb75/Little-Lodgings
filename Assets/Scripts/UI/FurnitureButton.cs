using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureButton : MonoBehaviour
{
    public Decorator decorator;
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
        }
    }

    void MessageDecorator()
    {
        decorator.CreatePlacingObject(furniture);
    }
}
