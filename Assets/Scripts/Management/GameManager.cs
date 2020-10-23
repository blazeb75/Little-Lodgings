using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool debugMode;

    public Camera freeCamera;

    [HideInInspector] public static Camera activeCam;
    public static bool speedMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("Duplicate GameManager found. Deleting...", this.gameObject);
        }
    }

    private void Start()
    {
        if(activeCam == null)
        {
            activeCam = Camera.main;
        }
    }

    public void SwitchCamera(Camera newCamera)
    {
        activeCam.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        activeCam = newCamera;
    }
}
