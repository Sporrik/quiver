using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SandwhichSpawner : MonoBehaviour
{
    [SerializeField] private float _initialSpeed = 5f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedMultiplier = 1.05f;
    [SerializeField] private int _spawnChance = 75;   // Chance to spawn an item in a sandwhich spawnpoint
    [SerializeField] private int _badSpawnChance = 50; // Chance that item spawned is bad
    [SerializeField] private int _goodItemValue = 5;
    [SerializeField] private int _badItemValue = 20;
    public bool IsCompleted = false;

    [SerializeField] private GameObject _sandwhichPrefab1;
    [SerializeField] private GameObject _sandwhichPrefab2;
    [SerializeField] private GameObject _sandwhichPrefab3;
    [SerializeField] private GameObject _sandwhichStart;
    [SerializeField] private TextMeshProUGUI _counter;
    [SerializeField] private GameObject _confettiParticle;
    [SerializeField] private GameObject _confettiParticle1;
    [SerializeField] private GameObject _confettiParticle2;
    [SerializeField] private GameObject _taskCompleted;
    [SerializeField] private GameObject _tip;



    [SerializeField] private List<GameObject> _badItems;
    [SerializeField] private List<GameObject> _goodItems;

    private int _percentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _speed = _initialSpeed;
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

        if (spawnPoint1 != null && Random.Range(0, 100) < _spawnChance /2f)
        {
            SpawnItemAt(spawnPoint1, sandwhich);
        }
        else if (spawnPoint2 != null && Random.Range(0, 100) < _spawnChance / 1.5f)
        {
            SpawnItemAt(spawnPoint2, sandwhich);
        }
        else if (spawnPoint3 != null && Random.Range(0, 100) < _spawnChance)
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

    public void EatItem(bool isBadItem)
    {
        if (!isBadItem)
        {
            _percentage += _goodItemValue;
            IncreaseSpeed();
            _counter.GetComponent<UIShake>().ShakeUI();

            if (_percentage >= 100)
            {
                _percentage = 100;
                IsCompleted = true;
                _confettiParticle.SetActive(true);
                _confettiParticle1.SetActive(true);
                _confettiParticle2.SetActive(true);
                _taskCompleted.SetActive(true);
                _counter.gameObject.SetActive(false);
                _tip.SetActive(false);
                Debug.Log("Hunger minigame completed!");
            }
        }
        else
        {
            if (IsCompleted)
                return;

            _percentage -= _badItemValue;

            if (_percentage < 0)
            {
                _percentage = 0;
            }

            DecreaseSpeed();
            _counter.GetComponent<UIShake>().ShakeUI();

        }

        _counter.text = _percentage.ToString() + " %";
    }

    private void IncreaseSpeed()
    {
        _speed *= _speedMultiplier;
        _speed = Mathf.Min(1.2f, _speed);
        GameObject[] sandwhiches = GameObject.FindGameObjectsWithTag("Sandwhich");
        foreach (GameObject sandwhich in sandwhiches)
        {
            sandwhich.GetComponent<Sandwhich>().Speed = _speed;
        }
    }

    private void DecreaseSpeed()
    {
        _speed /= _speedMultiplier;
        _speed /= _speedMultiplier;
        _speed /= _speedMultiplier;
        _speed /= _speedMultiplier;

        if(_speed < _initialSpeed)
            _speed = _initialSpeed;

        GameObject[] sandwhiches = GameObject.FindGameObjectsWithTag("Sandwhich");
        foreach (GameObject sandwhich in sandwhiches)
        {
            sandwhich.GetComponent<Sandwhich>().Speed = _speed;
        }
    }
}
