using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Poops;
    [SerializeField] private float SpawnMultiplier = 0.75f;
    public int CurrentPoops;
    [SerializeField] private bool AllPoopCleaned = false;
    [SerializeField] private bool BabyIsFlipped = false;
    public bool TaskCompleted = false;
    private int _amountToSpawn;

    [SerializeField] private DiaperChangingBehavior DirtyDiaper;
    [SerializeField] private DiaperChangingBehavior CleanDiaper;
    [SerializeField] private GameObject Baby;

    [SerializeField] private bool DirtyDiaperCompleted = false;
    public bool CleanDiaperEquipped = false;
    [SerializeField] private bool CleanDiaperCompleted = false;
    private bool hasMovedDirtyDiaper = false;
    private bool firstFlipTipShown = false;
    private bool hasShownCleanDiaperTip = false;
    private bool hasShownFlip2Tip = false;

    [SerializeField] private List<GameObject> _UITips;

    [Header("Win Condition:")]
    [SerializeField] private MinigameWinToggle _winToggle;
    [SerializeField] private float _timeUntilQuit = 3f;

    private bool _won = false;

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

        //Open diaper check
        if (DirtyDiaper.GetComponent<Animator>().GetBool("frontIsWorn") == false
            && DirtyDiaper.GetComponent<Animator>().GetBool("leftIsWorn") == false
            && DirtyDiaper.GetComponent<Animator>().GetBool("rightIsWorn") == false &&DirtyDiaperCompleted == false)
        {
            DirtyDiaperCompleted = true;
            SetActiveTip(_UITips[1]);
        }

        //Clean diaper check
        if (CleanDiaper.GetComponent<Animator>().GetBool("frontIsWorn") == true
            && CleanDiaper.GetComponent<Animator>().GetBool("leftIsWorn") == true
            && CleanDiaper.GetComponent<Animator>().GetBool("rightIsWorn") == true)
        {
            CleanDiaperCompleted = true;
        }

        if(Baby.GetComponent<FlipBaby>().isFlipped == true && AllPoopCleaned == false && firstFlipTipShown == false)
        {
            SetActiveTip(_UITips[2]);
            firstFlipTipShown = true;
        }

        if (DirtyDiaperCompleted && !hasMovedDirtyDiaper)
        {
            hasMovedDirtyDiaper = true;
            DirtyDiaper.GetComponent<DiaperChangingBehavior>().enabled = false;
            Baby.GetComponent<CapsuleCollider>().enabled = true;
            Baby.GetComponent<FlipBaby>().enabled = true;

            StartCoroutine(DelayedMoveTo());
        }

        // Check clean diaper equipped
        if (AllPoopCleaned && DirtyDiaperCompleted == true && TaskCompleted == false && hasShownFlip2Tip == false)
        {
            SetActiveTip(_UITips[1]);
            hasShownFlip2Tip = true;
        }

        // Win condition
        if (AllPoopCleaned && DirtyDiaperCompleted == true && CleanDiaperCompleted == true && TaskCompleted == false)
        {
            TaskCompleted = true;
            SetActiveTip(_UITips[5]);

            _won = true;
        }

        // Check if the baby is facing the camera
        if (AllPoopCleaned && Baby.transform.rotation.eulerAngles == new Vector3(270f, 0f, 0f) && BabyIsFlipped == false &&BabyIsFlipped==false)
        {
            Baby.GetComponent<FlipBaby>().enabled = false;
            Baby.GetComponent<CapsuleCollider>().enabled = false;
            CleanDiaper.GetComponent<DragDiaper>().enabled = true;
            BabyIsFlipped = true;
            SetActiveTip(_UITips[3]);
        }

        // Enable closing the clean diaper
        if (CleanDiaperEquipped && hasShownCleanDiaperTip == false)
        {
            CleanDiaper.GetComponent<BoxCollider>().enabled = false;
            CleanDiaper.GetComponent<DiaperChangingBehavior>().enabled = true;
            SetActiveTip(_UITips[4]);
            hasShownCleanDiaperTip = true;
        }

        if(_won)
        {
            if (_timeUntilQuit <= 0)
            {
                _winToggle.WinMinigame();
            }
            
            _timeUntilQuit -= Time.deltaTime;
            
        }
    }

    private IEnumerator DelayedMoveTo()
    {
        yield return new WaitForSeconds(1f);

        DirtyDiaper.gameObject.GetComponent<MoveToObject>().MoveTo(0);
    }

    private void SetActiveTip(GameObject tipToEnable)
    {
        // Disable all GameObjects in the _UITips list
        foreach (GameObject tip in _UITips)
        {
            tip.SetActive(false);
        }

        // Enable the specified GameObject
        if (tipToEnable != null)
        {
            tipToEnable.SetActive(true);
        }
    }
}
