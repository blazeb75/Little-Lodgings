using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChonkPreferences : MonoBehaviour
{
    public FavouriteFurniture[] favouriteFurnitures;
    [HideInInspector] public List<Furniture> furnitures;
    private void Start()
    {
        foreach (FavouriteFurniture furniture in favouriteFurnitures)
        {
            if (FurnitureDatabase.instance.TryGetFurniture(furniture.furnitureName, out Furniture furn)) 
            {
                furnitures.Add(furn);
            }
            else
            {
                Debug.LogError($"A chonk's favourite furniture {furniture.furnitureName} doesn't exist in the furniture database");
                furnitures.Add(null);
            }
        }
    }
}

public struct FavouriteFurniture
{
    public string furnitureName;
    public string hint;
   // float multiplier;
}