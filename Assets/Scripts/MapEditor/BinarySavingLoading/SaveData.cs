using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TileDataListWrapper
{
    public List<TileDataStruct> tiles;

}

//TODO:  add text box where you ask for the file name, all files will be saved to mapSaves(or other name for file-map)  
//TODO:  chose to save to json or fbx or both

public class SaveData : MonoBehaviour
{

    //[SerializeField] private TileDataStruct tileData = new TileDataStruct();
    [SerializeField] private GameObject tileListObject;

    private List<TileDataStruct> _tilesList = new List<TileDataStruct>();

    private void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("save files", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

    }

    private void GetTiles()
    {
        for (var idx = 0; idx < tileListObject.transform.childCount; idx++)
        {
            TileDataStruct data;

            data.position = tileListObject.transform.GetChild(idx).position;
            data.rotation = tileListObject.transform.GetChild(idx).rotation;
            data.tagName = tileListObject.transform.GetChild(idx).tag;

            _tilesList.Add(data);
        }
    }

    public IEnumerator ShowSaveDialogCoroutine()
    {
        string filePath = "mapSaves/";
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, filePath, null, "Save map", "Save");

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result);
    }
    private void OnFilesSelected(string[] filePaths)
    {
        GetTiles();

        if (!Directory.Exists("mapSaves")) Directory.CreateDirectory("mapSaves");

        var wrapper = new TileDataListWrapper();
        wrapper.tiles = _tilesList;

        string data = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(filePaths[0], data);
    }

    public void SaveToFile()
    {
        StartCoroutine(ShowSaveDialogCoroutine());
    }
}
