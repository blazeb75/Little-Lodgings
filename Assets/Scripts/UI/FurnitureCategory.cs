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

    private Transform emptyChild;

    private string Path
    {
        get
        {
            if (pathOverride == "")
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
        Texture2D[] thumbnails = bundle.LoadAllAssets<Texture2D>().Where(x => x.name.Contains(label)).ToArray();
        bundle.Unload(false);
        emptyChild = transform.Find("ScrollHolder");

        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            Texture2D thumbnail = thumbnails.Where(x => x.name == prefabs[i].name).First();float offset = -500 + (500 * i);
            CreateFurnitureButton(prefab, Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f)), offset);
            
        }
    }

    public void CreateFurnitureButton(GameObject furniture, Sprite thumbnail, float offset)
    {
        GameObject button = Instantiate(furnitureButtonPrefab);
        button.transform.SetParent(emptyChild, false);
        button.GetComponent<FurnitureButton>().furniture = furniture;
        Image image = button.transform.Find("Thumbnail").GetComponent<Image>();
        image.sprite = thumbnail;
        (button.transform as RectTransform).anchoredPosition = new Vector2(offset, 0);
        button.name = furniture.name;
        button.GetComponentInChildren<Text>().text = button.name;
        
    }
}
