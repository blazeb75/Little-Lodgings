using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
[Serializable]
public static class FurnitureDatabase
{
    private static GameObject furnitureBase;
    public static List<GameObject> furniturePrefabs = new List<GameObject>();
    public static List<string> furniturePaths = new List<string>();

    static FurnitureDatabase()
    {
        Refresh();
    }

    private static void Refresh()
    {
        furnitureBase = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/FurnitureBase.prefab");
    }

    [UnityEditor.MenuItem("Automation/Reload All Furniture")]
    public static void LoadAll()
    {
        Debug.ClearDeveloperConsole();

        Camera screenshotCam = null;
        List<string> thumbnailPaths = new List<string>();
        if (furnitureBase == null)
        {
            Refresh();
        }
        try
        {
            AssetDatabase.StartAssetEditing();
            screenshotCam = ((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Back-end/Screenshot Camera.prefab"))).GetComponent<Camera>();
            //screenshotCam.transform.position = new Vector3(995, 1005.67f, 995);
            
            Clear();
            TextAsset csvFile = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/furniture.csv");
            //string[][] table = CSVReader.SplitCsvGrid(csvFile.text);
            DataTable table = CSVReader.CsvToTable(csvFile);
            Debug.Log("Loading " + table.Rows.Count + " furniture prefabs");
            foreach (DataRow row in table.Rows)
            {
                GameObject dummyInstance = (GameObject)PrefabUtility.InstantiatePrefab(furnitureBase);
                string name = row.Field<string>("Furniture Name");
                dummyInstance.name = name;

                Furniture furniture = dummyInstance.GetComponent<Furniture>();
                furniture.cost = row.Field<float>("Money Cost");
                furniture.unlockCost = float.Parse(row.Field<string>("Prestige Unlock"));
                furniture.size = new Vector2(row.Field<float>("Size.x"), row.Field<float>("Size.y"));
                furniture.offset = new Vector3(row.Field<float>("Offset.x"), row.Field<float>("Offset.y"), row.Field<float>("Offset.z"));

                string[] tagColumns = { "Aesthetic", "Type"/*, "Room" */};
                List<Furniture.Tags> tags = new List<Furniture.Tags>();
                foreach (string header in tagColumns)
                {
                    string value = row.Field<string>(header);
                    if (Enum.TryParse(value, true, out Furniture.Tags tag))
                    {
                        tags.Add(tag);
                    }
                    else
                    {
                        Debug.LogError("Tag " + value + " is not defined ('" + header + "' of '" + name + "')");
                    }
                }
                furniture.tags = tags.ToArray();

                //Add model & texture
                string modelPath = "Assets/Models/Furniture/" + row.Field<string>("Aesthetic") + "/" + name/*.Replace(" ", string.Empty)*/+ ".fbx";
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (model == null)
                {
                    Debug.LogWarning("Model not found at " + modelPath);
                }
                else
                {
                    //Apply model
                    List<Material> mats = new List<Material>();
                    GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(model, dummyInstance.transform);
                    GameObject screenshotModel = (GameObject)PrefabUtility.InstantiatePrefab(model);
                    modelInstance.layer = LayerMask.NameToLayer("Furniture");
                    for (int i = 1; i <= 2; i++)
                    {
                        if (row.Field<string>("Base Material " + i.ToString()) == "")
                        {
                            continue;
                        }
                        else
                        {
                            string matBasePath = "Assets/Models/Materials/" + row.Field<string>("Base Material " + i) + ".mat";
                            Material matBase = AssetDatabase.LoadAssetAtPath<Material>(matBasePath);
                            if (matBase == null)
                            {
                                Debug.LogError(matBasePath + " returned no material");
                                continue;
                            }
                            Material mat = new Material(matBase);
                            mat.name = name + i;
                            string texturePath = "Assets/Models/Furniture/" + row.Field<string>("Aesthetic") + "/Materials and Textures/" + row.Field<string>("Texture " + i) + ".png";
                            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(texturePath);
                            if (texture == null)
                            {
                                Debug.LogError("Texture not found at " + texturePath);
                            }
                            mat.SetTexture("_MainTex", texture);
                            string matAssetPath = "Assets/Prefabs/Auto/Materials/" + mat.name + ".mat";
                            AssetDatabase.CreateAsset(mat, matAssetPath);
                            Material matAsset = AssetDatabase.LoadAssetAtPath<Material>(matAssetPath);
                            mats.Add(matAsset);
                        }
                    }
                    modelInstance.GetComponent<Renderer>().materials = mats.ToArray();
                    screenshotModel.GetComponent<Renderer>().materials = mats.ToArray();

                    //Create thumbnail image
                    RenderTexture rt = new RenderTexture(256, 256, 0);
                    screenshotCam.targetTexture = rt;
                    RenderTexture.active = rt;
                    screenshotModel.transform.position = new Vector3(999, 999, 999);
                    screenshotModel.transform.Rotate(0, 180, 0);
                    screenshotModel.layer = LayerMask.NameToLayer("Furniture");
                    //screenshotCam.orthographicSize = Mathf.Max(furniture.size.x, furniture.size.y);
                    screenshotCam.Render();
                    Texture2D image = new Texture2D(256, 256);
                    image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    image.Apply();
                    var Bytes = image.EncodeToPNG();
                    UnityEngine.Object.DestroyImmediate(image);
                    UnityEngine.Object.DestroyImmediate(screenshotModel);
                    string path = UnityEngine.Application.dataPath + "/Prefabs/Auto/Furniture/Thumbnails/" + name + ".png";
                    string shortPath =  "Assets/Prefabs/Auto/Furniture/Thumbnails/" + name + ".png";
                    File.WriteAllBytes(path, Bytes);

                    thumbnailPaths.Add(shortPath);

                    RenderTexture.active = null;
                    screenshotCam.targetTexture = null;
                    UnityEngine.Object.DestroyImmediate(rt);
                }

                string folderPath = "Assets/Prefabs/Auto/Furniture/" + row.Field<string>("Aesthetic");
                if (!Directory.Exists(folderPath))//!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder("Assets/Prefabs/Auto/Furniture", row.Field<string>("Aesthetic"));
                    AssetDatabase.Refresh();
                }
                string completePath = folderPath + "/" + name + ".prefab";
                GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(dummyInstance, completePath);
                GameObject.DestroyImmediate(dummyInstance);

                furniturePrefabs.Add(newPrefab);
                furniturePaths.Add(completePath);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            RenderTexture.active = null;
            GameObject.DestroyImmediate(screenshotCam.gameObject);
        }
        //Import sprites
        AssetDatabase.Refresh();
        foreach (string path in thumbnailPaths)
        {
            //Import thumbnails as sprites
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }

        //Build asset bundle
        AssetBundleBuild buildMap = new AssetBundleBuild();
        buildMap.assetNames = furniturePaths.Concat(thumbnailPaths).ToArray();
        buildMap.assetBundleName = "Furniture Prefabs";
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", new AssetBundleBuild[]{ buildMap }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        
        Debug.Log("Furniture refresh succeeded");
    }

    public static void Clear()
    {
        furniturePrefabs.Clear();
        furniturePaths.Clear();
        List<string> failedPaths = new List<string>();
        AssetDatabase.DeleteAssets(new string[]{"Assets/Prefabs/Auto/Furniture"}, failedPaths);
        AssetDatabase.DeleteAssets(new string[]{"Assets/Prefabs/Auto/Materials"}, failedPaths);
        AssetDatabase.CreateFolder("Assets/Prefabs/Auto", "Furniture");
        AssetDatabase.CreateFolder("Assets/Prefabs/Auto", "Materials");
        AssetDatabase.CreateFolder("Assets/Prefabs/Auto/Furniture", "Thumbnails");
        AssetDatabase.Refresh();
    }

}
#endif

