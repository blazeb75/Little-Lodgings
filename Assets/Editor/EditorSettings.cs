using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class EditorSettings
{
    public static bool FastStays;
    private const string menuFastStays = "DevMode/FastStays";

    static EditorSettings()
    {
        FastStays = EditorPrefs.GetBool(menuFastStays, true);
    }

    [MenuItem(menuFastStays)]
    private static void ToggleAction()
    {
        FastStays = !FastStays;
        EditorPrefs.SetBool(menuFastStays, FastStays);
    }

    [MenuItem(menuFastStays, true)]
    private static bool ToggleActionValidate()
    {
        Menu.SetChecked(menuFastStays, FastStays);
        return true;
    }
}
