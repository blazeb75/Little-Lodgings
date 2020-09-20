using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
#if UNITY_EDITOR
    static Vector2 lastMousePosition;
#endif

    static float priorPinch;

    private static float deltaPinch;

    public static bool Tap()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.tapCount == 1)
            {
                return true;
            }
        }
        return false;
    }
    private void Update()
    {
        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);

            deltaPinch = priorPinch - distance;
        }
    }

    private void LateUpdate()
    {
        lastMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float distance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            distance = Vector2.Distance(touch0, touch1);

            priorPinch = distance;
        }
    }


    //This should really just return 1 thing and not a list
    static List<Vector2> list = new List<Vector2>();
    public static IEnumerable<Vector2> Drags()
    {        
        list.Clear();
       
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
        {
            Vector2 newPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 deltaPos = lastMousePosition - newPos;
            deltaPos.x = deltaPos.x / Screen.width;
            deltaPos.y = deltaPos.y / Screen.height;
            list.Add(deltaPos);
        }
#endif
        if (Input.touchCount != 1)
        {
            return list;
        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 deltaPos = touch.deltaPosition;
                deltaPos.x = deltaPos.x / Screen.width;
                deltaPos.y = deltaPos.y / Screen.height;
                list.Add(deltaPos);
            }
        }
        return list;
    }

    public static float Pinch()
    {
        return deltaPinch;
    }

}


