using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestManager : MonoBehaviour
{
    public static GuestManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("Duplicate GuestManager found. Deleting...", this.gameObject);
        }
    }

    public GameObject[] ChonkPrefabs;

    public List<GameObject> ChonkInstances;
    public List<Stay> currentStays;
    public List<Stay> completedStays;

    public List<Room> rooms;

    public void SpawnChonk(GameObject prefab)
    {

    }

    public Stay GenerateStay(ChonkData chonk)
    {
        Stay stay = new Stay();
        stay.guestName = chonk.name;
        return stay;
    }

    public GameObject RandomChonk()
    {
        string[] presentChonks = currentStays.Select<Stay, string>(x => x.guestName).ToArray();
        //Narrow down options to chonks who are not already present
        GameObject[] validPrefabs = ChonkPrefabs.Where(prefab => !presentChonks.Where(name => name == prefab.name).Any()).ToArray();

        int index = UnityEngine.Random.Range(0, validPrefabs.Length);
        return validPrefabs[index];
    }

    public Room RandomRoom()
    {
        Room[] validRooms = rooms.Where(room => !currentStays.Where(stay => stay.roomID == room.roomID).Any()).ToArray();
        int index = UnityEngine.Random.Range(0, validRooms.Length);
        return validRooms[index];
    }

    public void CompleteStay(Stay stay)
    {
        currentStays.Remove(stay);
        completedStays.Add(stay);
    }
}

[Serializable]
public struct Stay
{
    public string guestName;
    public int roomID;
    public DateTime checkInTime;
    public DateTime checkOutTime;
}

