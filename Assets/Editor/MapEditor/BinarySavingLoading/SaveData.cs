using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEditor.Formats.Fbx.Exporter;

[System.Serializable]
public class TileDataListWrapper
{
    public List<TileDataStruct> tiles;

}

//TODO:  chose to save to json or fbx or both

public class SaveData : MonoBehaviour
{

    //[SerializeField] private TileDataStruct tileData = new TileDataStruct();
    [SerializeField] private GameObject tileListObject;

    private List<TileDataStruct> _tilesList = new List<TileDataStruct>();


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

    public IEnumerator ShowSaveDialogCoroutine(bool isFBX = false)
    {
        string filePath;
        if (isFBX)
        {
            filePath = "mapSaves/FBXSaves/";
            FileBrowser.SetFilters(true, new FileBrowser.Filter("save", ".fbx"));
            FileBrowser.SetDefaultFilter(".fbx");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe", ".json");
        }
        else
        {
            filePath = "mapSaves/";
            FileBrowser.SetFilters(true, new FileBrowser.Filter("save", ".json"));
            FileBrowser.SetDefaultFilter(".json");
            FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe", ".fbx");
        }

        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false, filePath, null, "Save map", "Save");

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result, isFBX);
    }
    private void OnFilesSelected(string[] filePaths, bool isFBX = false)
    {
        GetTiles();

        if (!Directory.Exists("mapSaves")) Directory.CreateDirectory("mapSaves");
        if (isFBX)
            if (!Directory.Exists("mapSaves/FBXSaves")) Directory.CreateDirectory("mapSaves/FBXSaves");


        var wrapper = new TileDataListWrapper();
        wrapper.tiles = _tilesList;

        //save to json file
        if (!isFBX)
        {
            string data = JsonUtility.ToJson(wrapper, true);
            System.IO.File.WriteAllText(filePaths[0], data);
        }
        else
        {
            //object array for fbx exporter
            Object[] map = new Object[tileListObject.transform.childCount];
            for (var idx = 0; idx < tileListObject.transform.childCount; idx++)
            {
                map[idx] = tileListObject.transform.GetChild(idx).gameObject;
            }

            ExportModelOptions exportSettings = new ExportModelOptions();
            exportSettings.ExportFormat = ExportFormat.Binary;
            exportSettings.KeepInstances = true;
            exportSettings.ModelAnimIncludeOption = Include.Model;

            ModelExporter.ExportObjects(filePaths[0], map, exportSettings);
        }
    }


    public void SaveToFile()
    {
        StartCoroutine(ShowSaveDialogCoroutine());
    }

    public void SaveToFBX()
    {
        StartCoroutine(ShowSaveDialogCoroutine(true));

    }
}
