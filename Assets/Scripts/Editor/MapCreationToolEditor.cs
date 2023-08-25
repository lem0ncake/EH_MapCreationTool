using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapCreationTool))]
public class MapCreationToolEditor : Editor
{
    bool isAuthorConfigurationVisible = true;
    bool isMapConfigurationVisible = true;
    GUIStyle customFoldoutStyle;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MapCreationTool tool = (MapCreationTool)target;
        customFoldoutStyle = EditorStyles.foldoutHeader;
        customFoldoutStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        customFoldoutStyle.fontSize = Screen.currentResolution.width / 130;

        EditorGUILayout.LabelField("Map Creation Tool", new GUIStyle()
        {
            fontSize = Screen.currentResolution.width / 70,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            normal = new GUIStyleState() { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
        });

        EditorGUILayout.Space(Screen.currentResolution.height / 192f);

        isAuthorConfigurationVisible = EditorGUILayout.BeginFoldoutHeaderGroup(isAuthorConfigurationVisible, "Author Configuration", customFoldoutStyle);
        if (isAuthorConfigurationVisible)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("author"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("email"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        isMapConfigurationVisible = EditorGUILayout.BeginFoldoutHeaderGroup(isMapConfigurationVisible, "Map Configuration", customFoldoutStyle);
        if (isMapConfigurationVisible)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("map"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(Screen.currentResolution.height / 96f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import"))
            tool.Import();
        if (GUILayout.Button("Export"))
            tool.Export();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Clear All"))
            tool.ClearAll();

        serializedObject.ApplyModifiedProperties();
    }
}