#if UNITY_EDITOR
using UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIScriptableObject))]
public sealed class UIScriptableObjectEditor : Editor
{
    // Serialized fields (defaults + mode)
    SerializedProperty _defaultsProp;
    SerializedProperty _defPoop;
    SerializedProperty _defPee;
    SerializedProperty _defHungry;
    SerializedProperty _defHappiness;
    SerializedProperty _defStamina;
    SerializedProperty _singlePlayer;

    void OnEnable()
    {

        
        _defaultsProp = serializedObject.FindProperty("_defaults");
        
        _defPoop      = _defaultsProp.FindPropertyRelative("Poop");
        _defPee       = _defaultsProp.FindPropertyRelative("Pee");
        _defHappiness = _defaultsProp.FindPropertyRelative("Happiness");
        _defHungry    = _defaultsProp.FindPropertyRelative("Hungry");
        _defStamina   = _defaultsProp.FindPropertyRelative("Stamina");
        _singlePlayer = serializedObject.FindProperty("_gameModeSinglePlayer");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var so = (UIScriptableObject)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Live Values (Runtime)", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(true))
        {
            DrawBar("Poop",      so.GetPoop());
            DrawBar("Pee",       so.GetPee());
            DrawBar("Hungry",    so.GetHungry());
            DrawBar("Happiness", so.GetHappiness());
            DrawBar("Stamina",   so.GetStamina());
        }
        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Defaults (Designer)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_defPoop);
        EditorGUILayout.PropertyField(_defPee);
        EditorGUILayout.PropertyField(_defHungry);
        EditorGUILayout.PropertyField(_defHappiness);
        EditorGUILayout.PropertyField(_defStamina);

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(_singlePlayer, new GUIContent("Game Mode Single Player"));

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Reset All To Defaults"))
            {
                Undo.RecordObject(so, "Reset UI Bars To Defaults");
                so.ResetAllToDefaults();
                EditorUtility.SetDirty(so);
            }
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Quick Nudge (for testing)", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+5 Poop")) { Undo.RecordObject(so, "Inc Poop"); so.IncrementPoop(5f); EditorUtility.SetDirty(so); }
            if (GUILayout.Button("-5 Poop")) { Undo.RecordObject(so, "Dec Poop"); so.IncrementPoop(-5f); EditorUtility.SetDirty(so); }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+5 Pee")) { Undo.RecordObject(so, "Inc Pee"); so.IncrementPee(5f); EditorUtility.SetDirty(so); }
            if (GUILayout.Button("-5 Pee")) { Undo.RecordObject(so, "Dec Pee"); so.IncrementPee(-5f); EditorUtility.SetDirty(so); }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+5 Hungry")) { Undo.RecordObject(so, "Inc Hungry"); so.IncrementHungry(5f); EditorUtility.SetDirty(so); }
            if (GUILayout.Button("-5 Hungry")) { Undo.RecordObject(so, "Dec Hungry"); so.IncrementHungry(-5f); EditorUtility.SetDirty(so); }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+5 Happy")) { Undo.RecordObject(so, "Inc Happy"); so.IncrementHappiness(5f); EditorUtility.SetDirty(so); }
            if (GUILayout.Button("-5 Happy")) { Undo.RecordObject(so, "Dec Happy"); so.IncrementHappiness(-5f); EditorUtility.SetDirty(so); }
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Set Stamina 100")) { Undo.RecordObject(so, "Stamina 100"); so.SetStamina(100f); EditorUtility.SetDirty(so); }
            if (GUILayout.Button("Set Stamina 0")) { Undo.RecordObject(so, "Stamina 0"); so.SetStamina(0f); EditorUtility.SetDirty(so); }
        }

        serializedObject.ApplyModifiedProperties();
        Repaint(); // keep live bars fresh in editor
    }

    private void DrawBar(string label, float value01_100)
    {
        float v01 = Mathf.Clamp01(value01_100 * 0.01f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(90));
        Rect r = GUILayoutUtility.GetRect(1, 16);
        EditorGUI.ProgressBar(r, v01, $"{value01_100:0.#}%");
        EditorGUILayout.EndHorizontal();
    }
}
#endif
