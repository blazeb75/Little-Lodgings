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

    private Transform content;

    public GameObject[] prefabs;

    private string Path
    {
        get
        {
            if (pathOverride == "")
            {
                return $"Auto/Furniture/{label}";
                //return "Assets/AssetBundles/furniture prefabs";
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
        //AssetBundle bundle = AssetBundle.LoadFromFile(Path);
        //GameObject[] prefabs = bundle.LoadAllAssets<GameObject>().Where(x => x.name.Contains(label)).ToArray();
        //Texture2D[] thumbnails = bundle.LoadAllAssets<Texture2D>().Where(x => x.name.Contains(label)).ToArray();
        //bundle.Unload(false);
        prefabs = Resources.LoadAll<GameObject>(Path);
        Texture2D[] thumbnails = Resources.LoadAll<Texture2D>(Path);

        content = transform.Find("Viewport").Find("Content");
        int i;
        for (i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            Texture2D thumbnail = thumbnails.Where(x => x.name == prefabs[i].name).First();
            float offset = -500 + (500 * i);
            CreateFurnitureButton(prefab, Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0.5f, 0.5f)), offset);

        }
        (content.transform as RectTransform).sizeDelta = new Vector2(i * 500, 400);
    }

    public void CreateFurnitureButton(GameObject furniture, Sprite thumbnail, float offset)
    {
        GameObject button = Instantiate(furnitureButtonPrefab);
        button.transform.SetParent(content, false);
        button.GetComponent<FurnitureButton>().furniture = furniture;
        Image image = button.transform.Find("Thumbnail").GetComponent<Image>();
        image.sprite = thumbnail;
        (button.transform as RectTransform).anchoredPosition = new Vector2(offset, 0);
        button.name = furniture.name;
        button.GetComponentInChildren<Text>().text = button.name;

    }
}
