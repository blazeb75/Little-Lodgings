using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public static bool Rotate()
    {
        if (Input.GetKeyDown(KeyCode.R))
            return true;
        else return false;
    }
    public static bool Accept()
    {
        if (Input.GetKeyDown(KeyCode.E))
            return true;
        else return false;
    }
}
