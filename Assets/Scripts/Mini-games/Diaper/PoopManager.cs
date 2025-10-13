using System.Collections.Generic;
using UnityEngine;

public class PoopManager : MonoBehaviour
{
    public List<GameObject> Poops;
    public float SpawnMultiplier = 0.75f;
    public int CurrentPoops;
    public bool AllPoopCleaned = false;
    public bool TaskCompleted = false;
    private int _amountToSpawn;
    public LockBones LockPosition1;
    public LockBones LockPosition2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _amountToSpawn = (int)(Poops.Count * SpawnMultiplier);
        for (int i = 0; i < _amountToSpawn; i++)
        {
            int index = Random.Range(0, Poops.Count);
            Poops[index].SetActive(true);
            Poops.RemoveAt(index);
        }

        CurrentPoops = _amountToSpawn;

    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentPoops <= 0 && AllPoopCleaned == false)
        {
            Debug.Log("All Poops Cleaned!");
            AllPoopCleaned = true;
        }

        if(AllPoopCleaned && LockPosition1.IsLocked && LockPosition2.IsLocked && TaskCompleted == false)
        {
            Debug.Log("Task Completed!");
            TaskCompleted = true;
            // You can add additional logic here for when the task is completed
        }
    }
}
