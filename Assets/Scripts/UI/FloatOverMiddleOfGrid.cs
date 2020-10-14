using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatOverMiddleOfGrid : MonoBehaviour
{
    public PlacementGrid grid;
    public float height;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = grid.GetCenter();
        transform.Translate(0, height, 0);

        Decorator.instance.OnEnterEditMode.AddListener(delegate { gameObject.SetActive(false); });
        Decorator.instance.OnExitEditMode.AddListener(delegate { gameObject.SetActive(true); });
    }
}
