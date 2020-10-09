using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

[Serializable]
public static class FurnitureDatabase
{
    private static GameObject furnitureBase;
    public static List<GameObject> furniturePrefabs = new List<GameObject>();

    static FurnitureDatabase()
    {
        Refresh();
    }

    private static void Refresh()
    {
        furnitureBase = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Furniture/FurnitureBase.prefab");
    }

    [MenuItem("Automation/Reload All Furniture")]
    public static void LoadAll()
    {
        if(furnitureBase == null)
        {
            Refresh();
        }
        try
        {
            AssetDatabase.StartAssetEditing();
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
                try
                {
                    furniture.cost = float.Parse(row.Field<string>("Money Cost"));
                }
                catch
                {
                    Debug.LogError("Value of " + row.Field<string>("Money Cost") + " could not be parsed for Money Cost of " + name);
                }
                furniture.unlockCost = float.Parse(row.Field<string>("Prestige Unlock"));

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
                string modelPath = "Assets/Models/Furniture/" + row.Field<string>("Aesthetic") + "/" + name/*.Replace(" ", string.Empty)*/+".fbx";
                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (model == null)
                {
                    Debug.LogWarning("Model not found at " + modelPath);
                }
                else
                {
                    List<Material> mats = new List<Material>(); 
                    GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(model, dummyInstance.transform);
                    for (int i = 1; i <= 2; i++)
                    {
                        if (row.Field<string>("Base Material {i}") == "")
                        {
                            continue;
                        }
                        else
                        {                            
                            Material matBase = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/" + row.Field<string>("Base Material {i}") + ".mat");
                            Material mat = new Material(matBase);
                            mat.name = name + i;
                            string texturePath = "Assets/Models/Furniture/" + row.Field<string>("Aesthetic") + "/Materials and Textures/" + row.Field<string>("Texture {i}") + ".png";
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
                }

                string folderPath = "Assets/Prefabs/Auto/Furniture/" + row.Field<string>("Aesthetic");
                if (!System.IO.Directory.Exists(folderPath))//!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder("Assets/Prefabs/Auto/Furniture", row.Field<string>("Aesthetic"));                    
                    AssetDatabase.Refresh();
                }
                GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(dummyInstance, folderPath + "/" + name + ".prefab");
                GameObject.DestroyImmediate(dummyInstance);

                furniturePrefabs.Add(newPrefab);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }

    public static void Clear()
    {
        furniturePrefabs.Clear();
        List<string> failedPaths = new List<string>();
        AssetDatabase.DeleteAssets(new string[]{"Assets/Prefabs/Auto/Furniture"}, failedPaths);
        AssetDatabase.CreateFolder("Assets/Prefabs/Auto", "Furniture");
        AssetDatabase.CreateFolder("Assets/Prefabs/Auto", "Materials");
        AssetDatabase.Refresh();
    }

}

    
