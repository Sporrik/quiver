#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEngine;
using SimpleFileBrowser;

public class LoadData : MonoBehaviour
{
    private TileDataListWrapper _tilesInJson = null;
    private bool _fileSelected = false;

    public IEnumerator ShowLoadDialogCoroutine()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("load", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        string filePath = "mapSaves/";
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, filePath, null, "Select map", "Load");

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result);
    }

    private void OnFilesSelected(string[] filePaths)
    {
        string filePath = FileBrowser.Result[0];

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            _tilesInJson = JsonUtility.FromJson<TileDataListWrapper>(jsonString);

            _fileSelected = true;
        }
        else
        {
            Debug.LogError("File not found at " + filePath);
        }
    }
    public void LoadFromFile()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public TileDataListWrapper GetLoadedList()
    {
        return _tilesInJson;
    }

    public bool GetFileSelected()
    {
        return _fileSelected;
    }

    public void ResetFileSelected()
    {
        _fileSelected = false;
    }
}
#endif