#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var ctrl = (PlayerController)target;
        var so = new SerializedObject(ctrl);
        var cfgProp = so.FindProperty("_playerCfg");

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = cfgProp.objectReferenceValue != null;

            if (GUILayout.Button("Edit Player Config"))
                Selection.activeObject = cfgProp.objectReferenceValue;

            GUI.enabled = true;

            if (GUILayout.Button("Ping"))
                EditorGUIUtility.PingObject(cfgProp.objectReferenceValue);
        }
    }
}
#endif