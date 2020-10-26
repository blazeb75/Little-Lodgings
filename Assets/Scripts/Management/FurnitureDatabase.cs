using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureDatabase : MonoBehaviour
{
    public static FurnitureDatabase instance;
    public Furniture[] furniture;
    public GameObject[] furniturePrefabs;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("Duplicate FurnitureDatabase found. Deleting...", this.gameObject);
        }
        furniturePrefabs = Resources.LoadAll<GameObject>("Auto/Furniture");
        furniture = furniturePrefabs.Select(x => x.GetComponent<Furniture>()).ToArray();
    }

    public GameObject GetPrefab(string name)
    {
        return furniturePrefabs.Where(x => x.name == name).First();
    }
    public bool TryGetPrefab(string name, out GameObject prefab)
    {
        prefab = furniturePrefabs.Where(x => x.name == name).FirstOrDefault();
        return prefab != null;
    }
    public bool TryGetFurniture(string name, out Furniture furnitureComponent)
    {
        furnitureComponent = furniture.Where(x => x.name == name).FirstOrDefault();
        return furnitureComponent != null;
    }
}
