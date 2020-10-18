using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChonkBehaviour : MonoBehaviour
{
    public enum State { WanderingInBedroom }
    public State state;
    public PlacementGrid room;

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WanderingInBedroom:
                WanderInBedroom();
                break;
        }
    }

    public void WanderInBedroom()
    {

    }
}
