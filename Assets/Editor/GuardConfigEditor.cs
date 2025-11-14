#if UNITY_EDITOR
using Gameplay.GuardCfg;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GuardConfig))]
public sealed class GuardConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cfg = (GuardConfig)target;
        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Guards Using This Config"))
            {
                var users = FindObjectsByType<Gameplay.AI.GuardBehavior>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                            .Where(g => g != null && GetConfig(g) == cfg)
                            .Cast<Object>()
                            .ToArray();

                if (users.Length > 0) Selection.objects = users;
                else EditorUtility.DisplayDialog("No Users", "No GuardBehavior found using this config in the open scenes.", "OK");
            }

            if (GUILayout.Button("Ping Selected Guards"))
            {
                foreach (var obj in Selection.objects)
                    EditorGUIUtility.PingObject(obj);
            }
        }

        if (GUILayout.Button("Apply This Config To Selected Guards"))
        {
            var guards = Selection.objects
                .OfType<GameObject>()
                .Select(go => go.GetComponent<Gameplay.AI.GuardBehavior>())
                .Where(g => g != null)
                .ToArray();

            if (guards.Length == 0)
            {
                EditorUtility.DisplayDialog("No Guards Selected", "Select Guard GameObjects first.", "OK");
                return;
            }

            Undo.RecordObjects(guards, "Assign GuardConfig");
            foreach (var g in guards)
            {
                var so = new SerializedObject(g);
                var prop = so.FindProperty("_guardCfg");
                if (prop != null)
                {
                    prop.objectReferenceValue = cfg;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(g);
                }
            }
        }
    }

    private GuardConfig GetConfig(Gameplay.AI.GuardBehavior g)
    {
        // Use SerializedObject to avoid needing public getters
        var so = new SerializedObject(g);
        var prop = so.FindProperty("_guardCfg");
        return prop != null ? prop.objectReferenceValue as GuardConfig : null;
    }
}
#endif