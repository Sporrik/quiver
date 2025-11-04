using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using SimpleFileBrowser;


//TODO:  Be able to select/insert the file you want to load 
//TODO:  Unload a map
public class LoadData : MonoBehaviour
{
    private TileDataListWrapper _tilesInJson = null;
    private bool _FileSelected = false;

    private void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("save files",".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

    }

    public IEnumerator ShowLoadDialogCoroutine()
    {
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

            _FileSelected = true;
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
        return _FileSelected;
    }

    public void ResetFileSelected()
    {
        _FileSelected = false;
    }
}
