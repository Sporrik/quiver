#if UNITY_EDITOR
using UI;
using UnityEditor;
using UnityEngine;

public static class UIMeterBatchEditor
{
    [MenuItem("Tools/UI/Convert Selected Hierarchy To Image-Fill Meters")]
    public static void ConvertSelectionToMeters()
    {
        var go = Selection.activeGameObject;
        if (go == null)
        {
            EditorUtility.DisplayDialog("No Selection", "Select a parent GameObject in the hierarchy.", "OK");
            return;
        }

        var binder = go.GetComponent<UIMeterBatchBinder>();
        if (binder == null)
        {
            binder = Undo.AddComponent<UIMeterBatchBinder>(go);
            Debug.Log("Added UIMeterBatchBinder to selection. Assign UIData, then click 'Apply Now' in the component.", go);
        }
        else
        {
            Debug.Log("UIMeterBatchBinder already present. Set UIData then click 'Apply Now'.", go);
        }
    }
}
#endif
