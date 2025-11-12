#if UNITY_EDITOR
using Gameplay.GuardCfg;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Gameplay.AI.GuardBehavior))]
public sealed class GuardBehaviorEditor : Editor
{
    private SerializedProperty _guardCfg;
    private SerializedProperty _takedown;
    private SerializedProperty _player;
    private SerializedProperty _eyes;
    private SerializedProperty _waypoints;
    private SerializedProperty _losMask;

    private void OnEnable()
    {
        _guardCfg = serializedObject.FindProperty("_guardCfg");
        _takedown = serializedObject.FindProperty("_takedown");
        _player = serializedObject.FindProperty("_player");
        _eyes = serializedObject.FindProperty("_eyes");
        _waypoints = serializedObject.FindProperty("_waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Designer Utilities", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Auto-Find Player"))
                AutoFindPlayerTag();

            if (GUILayout.Button("Create/Assign Eyes"))
                CreateAssignEyes();
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = _guardCfg?.objectReferenceValue != null;
            if (GUILayout.Button("Open Config"))
                Selection.activeObject = _guardCfg.objectReferenceValue;

            GUI.enabled = _takedown?.objectReferenceValue != null;
            if (GUILayout.Button("Open Takedown"))
                Selection.activeObject = _takedown.objectReferenceValue;

            GUI.enabled = true;
        }

        if (GUILayout.Button("Create Missing Configs"))
            CreateMissingConfigsNextToAsset();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Selected As Waypoints"))
                AddSelectedAsWaypoints();

            if (GUILayout.Button("Clear Waypoints"))
                ClearWaypoints();
        }

        if (GUILayout.Button("Validate Setup"))
            ValidateSetup();

        serializedObject.ApplyModifiedProperties();
    }

    private void AutoFindPlayerTag()
    {
        var go = ((Gameplay.AI.GuardBehavior)target).gameObject;
        Undo.RecordObject(go, "Auto-Find Player");
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _player.objectReferenceValue = player.transform;
            serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.PingObject(player);
        }
        else
        {
            EditorUtility.DisplayDialog("Player Not Found", "No GameObject with tag 'Player' was found.", "OK");
        }
    }

    private void CreateAssignEyes()
    {
        var guard = (Gameplay.AI.GuardBehavior)target;
        var root = guard.transform;
        var eyes = _eyes.objectReferenceValue as Transform;

        if (eyes == null)
        {
            var child = new GameObject("Eyes");
            Undo.RegisterCreatedObjectUndo(child, "Create Eyes");
            child.transform.SetParent(root, false);
            // Rough placement at head height
            child.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            child.transform.localRotation = Quaternion.identity;

            _eyes.objectReferenceValue = child.transform;
            serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.PingObject(child);
        }
        else
        {
            EditorGUIUtility.PingObject(eyes);
        }
    }

    private void CreateMissingConfigsNextToAsset()
    {
        var guard = (Gameplay.AI.GuardBehavior)target;

        string basePath = GetPreferredAssetFolder(guard.gameObject);
        if (string.IsNullOrEmpty(basePath)) basePath = "Assets";

        var so = serializedObject;
        bool changed = false;

        if (_guardCfg.objectReferenceValue == null)
        {
            var cfg = ScriptableObject.CreateInstance<GuardConfig>();
            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(basePath, $"GuardConfig_From_{guard.name}.asset"));
            AssetDatabase.CreateAsset(cfg, path);
            _guardCfg.objectReferenceValue = cfg;
            changed = true;
        }

        if (_takedown != null && _takedown.objectReferenceValue == null)
        {
            var tk = ScriptableObject.CreateInstance<TakedownConfig>();
            string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(basePath, $"Takedown_From_{guard.name}.asset"));
            AssetDatabase.CreateAsset(tk, path);
            _takedown.objectReferenceValue = tk;
            changed = true;
        }

        if (changed)
        {
            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(guard);
        }
    }

    private void AddSelectedAsWaypoints()
    {
        if (_waypoints == null) return;

        var selected = Selection.transforms;
        if (selected == null || selected.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Select transforms to add as waypoints.", "OK");
            return;
        }

        Undo.RecordObject(target, "Add Waypoints");
        foreach (var t in selected)
        {
            if (t == null) continue;
            int newIndex = _waypoints.arraySize;
            _waypoints.InsertArrayElementAtIndex(newIndex);
            var element = _waypoints.GetArrayElementAtIndex(newIndex);
            element.objectReferenceValue = t;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void ClearWaypoints()
    {
        if (_waypoints == null) return;
        if (!EditorUtility.DisplayDialog("Clear Waypoints", "Remove all waypoints from this guard?", "Yes", "No"))
            return;

        Undo.RecordObject(target, "Clear Waypoints");
        _waypoints.ClearArray();
        serializedObject.ApplyModifiedProperties();
    }

    private void ValidateSetup()
    {
        var guard = (Gameplay.AI.GuardBehavior)target;

        var issues = new System.Text.StringBuilder();
        var so = serializedObject;

        if (_guardCfg == null || _guardCfg.objectReferenceValue == null)
            issues.AppendLine("• Config is missing.");

        if (_eyes == null || _eyes.objectReferenceValue == null)
            issues.AppendLine("• Eyes transform is missing.");

        if (_waypoints != null && _waypoints.arraySize == 0)
            issues.AppendLine("• No waypoints assigned (guard will idle).");

        // Check collider co-located for PlayerInteraction overlap.
        var col = guard.GetComponent<Collider>();
        if (col == null)
            issues.AppendLine("• No Collider on the same GameObject. PlayerInteraction overlap may not find ITakedownTarget.");

        if (issues.Length == 0)
            EditorUtility.DisplayDialog("Validation", "Looks good!", "OK");
        else
            EditorUtility.DisplayDialog("Validation", issues.ToString(), "OK");
    }

    private static string GetPreferredAssetFolder(GameObject go)
    {
        // Try prefab asset path
        var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
        if (!string.IsNullOrEmpty(path))
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) return dir;
        }

        // Try scene path folder
        if (go.scene.IsValid() && !string.IsNullOrEmpty(go.scene.path))
        {
            var dir = Path.GetDirectoryName(go.scene.path);
            if (!string.IsNullOrEmpty(dir)) return dir;
        }

        return "Assets";
    }
}
#endif