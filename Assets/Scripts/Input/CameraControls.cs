using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 30f;
    public float zoomFactor = 1f;
    public float minFov = 30f;
    public float maxFov = 60f;

    new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        //Drag
        if (Decorator.instance.SelectedObject == null)
        {
            foreach (Vector2 drag in InputHandler.Drags())
            {
                transform.Translate(new Vector3(drag.x, drag.y, 0) * moveSpeed * camera.fieldOfView / 30);
            }
        }

        //Pinch zoom
#if UNITY_IOS || UNITY_ANDROID
        camera.fieldOfView += InputHandler.Pinch() * zoomFactor;
#else
        camera.fieldOfView -= Input.mouseScrollDelta.y * zoomFactor;
#endif
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minFov, maxFov);
    }
}
