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

    public void SaveToFile()
    {
        GetTiles();

        if (!Directory.Exists("mapSaves")) Directory.CreateDirectory("mapSaves");

        var wrapper = new TileDataListWrapper();
        wrapper.tiles = _tilesList;

        string data = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText("mapSaves/mapData.json", data);
    }
}
