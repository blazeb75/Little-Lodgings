using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();
    //    if(GUILayout.Button("Refresh Furniture Database"))
    //    {
    //        FurnitureDatabase.LoadAll();
    //    }
    //}
}
