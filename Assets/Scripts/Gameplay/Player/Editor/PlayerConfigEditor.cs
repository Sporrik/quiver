#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(PlayerConfig))]
public sealed class PlayerConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cfg = (PlayerConfig)target;
        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = cfg.Movement != null;

            if (GUILayout.Button("Open Movement Config"))
                Selection.activeObject = cfg.Movement;

            GUI.enabled = cfg.Stamina != null;

            if (GUILayout.Button("Open Stamina Config"))
                Selection.activeObject = cfg.Stamina;

            GUI.enabled = true;
        }

        // Create sub-configs if missing (puts them next to this PlayerConfig asset)
        if (GUILayout.Button("Create Missing Sub-Configs"))
        {
            string cfgPath = AssetDatabase.GetAssetPath(cfg);
            string dir = string.IsNullOrEmpty(cfgPath) ? "Assets" : Path.GetDirectoryName(cfgPath);

            var so = new SerializedObject(cfg);
            var moveProp = so.FindProperty("_movement");
            var stamProp = so.FindProperty("_stamina");

            if (moveProp.objectReferenceValue == null)
            {
                var movement = ScriptableObject.CreateInstance<MovementConfig>();
                string mPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, "Movement_From_" + cfg.name + ".asset"));
                AssetDatabase.CreateAsset(movement, mPath);
                moveProp.objectReferenceValue = movement;
            }

            if (stamProp.objectReferenceValue == null)
            {
                var stamina = ScriptableObject.CreateInstance<StaminaConfig>();
                string sPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, "Stamina_From_" + cfg.name + ".asset"));
                AssetDatabase.CreateAsset(stamina, sPath);
                stamProp.objectReferenceValue = stamina;
            }

            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(cfg);
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Ping Movement")) EditorGUIUtility.PingObject(cfg.Movement);
            if (GUILayout.Button("Ping Stamina")) EditorGUIUtility.PingObject(cfg.Stamina);
        }
    }
}
#endif
