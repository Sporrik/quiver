using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EdibleItem : MonoBehaviour
{
    [SerializeField] private bool isBadItem = false;
    [SerializeField] private GameObject _baby;
    public SpriteRenderer ControlSprite;
    private EdiblesManager _ediblesManager;
    public int InputIndex;


    void Start()
    {
        _ediblesManager = FindFirstObjectByType<EdiblesManager>();
        _ediblesManager.DetectNextClosestItem();
        _baby = GameObject.Find("BabyModel");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ItemEater")
        {
            SandwhichSpawner sandwhichSpawner = FindFirstObjectByType<SandwhichSpawner>();
            sandwhichSpawner.EatItem(isBadItem);

            if(isBadItem)
            {
                _baby.GetComponent<IncorrectFood>().TriggerFlash();
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _ediblesManager.RemoveItemFromList(this);
        _ediblesManager.ClearInputIndex(this.InputIndex);
        _ediblesManager.DetectNextClosestItem();
    }
}
