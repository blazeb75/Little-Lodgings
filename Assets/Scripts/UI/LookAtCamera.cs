using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(GameManager.instance.freeCamera.transform.position);
        transform.forward = -transform.forward;
    }
}
