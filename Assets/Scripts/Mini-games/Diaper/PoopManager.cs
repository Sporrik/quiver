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

    public DiaperChangingBehavior DirtyDiaper;
    public DiaperChangingBehavior CleanDiaper;
    public GameObject Baby;

    public bool DirtyDiaperCompleted = false;
    public bool CleanDiaperCompleted = false;
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

        CleanDiaper.GetComponent<DiaperChangingBehavior>().enabled = false;
        CleanDiaper.GetComponent<Animator>().SetBool("frontIsWorn", false);
        CleanDiaper.GetComponent<Animator>().SetBool("leftIsWorn", false);
        CleanDiaper.GetComponent<Animator>().SetBool("rightIsWorn", false);

    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentPoops <= 0 && AllPoopCleaned == false)
        {
            Debug.Log("All Poops Cleaned!");
            AllPoopCleaned = true;
        }

        if(DirtyDiaper.GetComponent<Animator>().GetBool("frontIsWorn") == false
            && DirtyDiaper.GetComponent<Animator>().GetBool("leftIsWorn") == false
            && DirtyDiaper.GetComponent<Animator>().GetBool("rightIsWorn") == false)
        {
            DirtyDiaperCompleted = true;
        }

        if (CleanDiaper.GetComponent<Animator>().GetBool("frontIsWorn") == true
            && CleanDiaper.GetComponent<Animator>().GetBool("leftIsWorn") == true
            && CleanDiaper.GetComponent<Animator>().GetBool("rightIsWorn") == true)
        {
            CleanDiaperCompleted = true;
        }

        if (DirtyDiaperCompleted)
        {
            DirtyDiaper.GetComponent<BoxCollider>().enabled = true;
            DirtyDiaper.GetComponent<Rigidbody>().isKinematic = false;
            DirtyDiaper.GetComponent<Rigidbody>().useGravity = true;
            DirtyDiaper.GetComponent<TopDownDrag>().enabled = true;
            DirtyDiaper.GetComponent<DiaperChangingBehavior>().enabled = false;
            Baby.GetComponent<CapsuleCollider>().enabled = true;
            Baby.GetComponent<FlipBaby>().enabled = true;

        }

        if (AllPoopCleaned && DirtyDiaperCompleted == true && CleanDiaperCompleted == true && TaskCompleted == false)
        {
            Debug.Log("Task Completed!");
            TaskCompleted = true;
            // You can add additional logic here for when the task is completed
        }
    }
}
