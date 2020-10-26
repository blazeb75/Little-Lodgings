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
    GameObject spawnPoint;
    public GameObject[] ChonkPrefabs;

    public List<GameObject> ChonkInstances;
    public List<Stay> currentStays;
    public List<Stay> completedStays;

    public List<Room> rooms;

    public void BeginVisit(Stay stay, GameObject chonkPrefab)
    {
        GameObject chonk = SpawnChonk(chonkPrefab);        
        ChonkInstances.Add(chonk);
        currentStays.Add(stay);
    }

    public Stay GenerateRandomVisit(out GameObject chonkPrefab)
    {
        GameObject chonk = RandomChonk();
        Stay stay = GenerateStay(chonk.GetComponent<ChonkData>());
        chonkPrefab = chonk;
        return stay;
    }

    GameObject SpawnChonk(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation, transform);
        return newObj;
    }

    Stay GenerateStay(ChonkData chonk)
    {
        Stay stay = new Stay
        {
            guestName = chonk.name,
            roomID = RandomRoom().roomID,
            checkInTime = DateTime.MaxValue,
            checkOutTime = DateTime.MaxValue
        };
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

    Room RandomRoom()
    {
        Room[] validRooms = rooms.Where(room => !currentStays.Where(stay => stay.roomID == room.roomID).Any()).ToArray();
        int index = UnityEngine.Random.Range(0, validRooms.Length);
        return validRooms[index];
    }

    public void CompleteStay(Stay stay)
    {
        currentStays.Remove(stay);
        completedStays.Add(stay);
        stay.chonkInstance.state = ChonkBehaviour.State.Leaving;
    }

    public void CheckIn(Stay stay, TimeSpan duration)
    {
        stay.checkInTime = DateTime.Now;
        stay.Duration = duration;
    }
}

[Serializable]
public struct Stay
{
    public string guestName;
    public int roomID;
    public DateTime checkInTime;
    public DateTime checkOutTime;
    public ChonkBehaviour chonkInstance;
    public TimeSpan Duration 
    { 
        get
        {
            return checkOutTime - checkInTime;
        }
        set
        {
            checkOutTime = checkInTime + value;
        }
    }
    public TimeSpan RemainingDuration 
    { 
        get
        {
            return checkOutTime - DateTime.Now;
        } 
    }
}

