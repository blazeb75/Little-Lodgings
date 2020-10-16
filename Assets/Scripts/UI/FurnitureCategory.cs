using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureCategory : MonoBehaviour
{
    public string label;
    public string pathOverride = "";
    public GameObject furnitureButtonPrefab;

    private string Path 
    { 
        get
        {
            if(pathOverride == "")
            {
                //return "Assets/AssetBundles/" + label + ".unity3d";
                return "Assets/AssetBundles/furniture prefabs";
            }
            else
            {
                return pathOverride;
            }
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(Path);
        GameObject[] prefabs = bundle.LoadAllAssets<GameObject>().Where(x => x.name.Contains(label)).ToArray();
        Image[] thumbnails = bundle.LoadAllAssets<Image>().Where(x => x.name.Contains(label)).ToArray();
        foreach (Object prefab in prefabs)
        {

        }
    }

    public void CreateFurnitureButton(GameObject furniture)
    {

    }
}
