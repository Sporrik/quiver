using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SandwhichSpawner : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _spawnChance = 75;   // Chance to spawn an item in a sandwhich spawnpoint
    [SerializeField] private int _badSpawnChance = 50; // Chance that item spawned is bad

    [SerializeField] private GameObject _sandwhichPrefab1;
    [SerializeField] private GameObject _sandwhichPrefab2;
    [SerializeField] private GameObject _sandwhichPrefab3;
    [SerializeField] private GameObject _sandwhichStart;


    [SerializeField] private List<GameObject> _badItems;
    [SerializeField] private List<GameObject> _goodItems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sandwhichStart.GetComponent<Sandwhich>().Speed = _speed;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sandwhich"))
        {
            GameObject sandwhich = PickSandwhich();
            GameObject spawnedSandwhich = Instantiate(sandwhich, transform.position, transform.rotation);
            spawnedSandwhich.GetComponent<Sandwhich>().Speed = _speed;
            SpawnItems(spawnedSandwhich);
        }
    }

    private GameObject PickSandwhich()
    {
        GameObject sandwhichToReturn;
        int roll = Random.Range(0, 2);
        if(roll == 0)
        {
            sandwhichToReturn = _sandwhichPrefab1;
        }
        else if(roll == 1)
        {
            sandwhichToReturn = _sandwhichPrefab2;
        }
        else
        {
            sandwhichToReturn = _sandwhichPrefab3;
        }

        return  sandwhichToReturn;
    }

    private void SpawnItems(GameObject sandwhich)
    {

        Transform spawnPoint1 = sandwhich.transform.Find("SpawnPoint1");
        Transform spawnPoint2 = sandwhich.transform.Find("SpawnPoint2");
        Transform spawnPoint3 = sandwhich.transform.Find("SpawnPoint3");

        if (spawnPoint1 != null && Random.Range(0, 100) < _spawnChance)
        {
            SpawnItemAt(spawnPoint1, sandwhich);
        }
        if (spawnPoint2 != null && Random.Range(0, 100) < _spawnChance)
        {
            SpawnItemAt(spawnPoint2, sandwhich);
        }
        if (spawnPoint3 != null && Random.Range(0, 100) < _spawnChance)
        {
            SpawnItemAt(spawnPoint3, sandwhich);
        }
    }

    private void SpawnItemAt(Transform spawnPoint, GameObject parent)
    {
        if(Random.Range(0, 100)<=_badSpawnChance)         // bad item
        {
            int roll = Random.Range(0, _badItems.Count);
            Instantiate(_badItems[roll], spawnPoint.position, spawnPoint.rotation, parent.transform);
        }
        else                                              // good item
        {
            int roll = Random.Range(0, _goodItems.Count);
            Instantiate(_goodItems[roll], spawnPoint.position, spawnPoint.rotation, parent.transform);
        }
    }
}
