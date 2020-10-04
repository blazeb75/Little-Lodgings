using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatOverMiddleOfGrid : MonoBehaviour
{
    public PlacementGrid grid;
    public float height;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = grid.GetCenter();
        transform.Translate(0, height, 0);
    }
}
