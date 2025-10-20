using UnityEngine;
using System.IO;

//TODO:  Be able to select/insert the file you want to load 
//TODO:  Unload a map
public class LoadData : MonoBehaviour
{
    private TileDataListWrapper _tilesInJson = null;
    public void LoadFromFile()
    {
        string filePath = "mapSaves/mapData.json";

        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            _tilesInJson = JsonUtility.FromJson<TileDataListWrapper>(jsonString);

            foreach (var tile in _tilesInJson.tiles)
            {
                Debug.Log(tile.tagName + " - " + tile.position);
            }
        }
        else
        {
            Debug.LogError("File not found at " + filePath);
        }
    }

    public TileDataListWrapper GetLoadedList()
    {
        return _tilesInJson;
    }

}
