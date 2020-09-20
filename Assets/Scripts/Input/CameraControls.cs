using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float zoomFactor = 5f;
    public float minFov = 10f;
    public float maxFov = 60f;

    new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        //Drag
        foreach(Vector2 drag in InputHandler.Drags())
        {
            transform.Translate(new Vector3(drag.x, drag.y, 0) * moveSpeed);
        }

        //Pinch zoom
        camera.fieldOfView += InputHandler.Pinch() * zoomFactor;

        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minFov, maxFov);
    }
}
