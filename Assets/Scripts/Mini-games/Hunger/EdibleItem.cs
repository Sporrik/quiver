using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EdibleItem : MonoBehaviour
{
    [SerializeField] private bool isBadItem = false;
    public SpriteRenderer ControlSprite;
    private EdiblesManager _ediblesManager;
    public int InputIndex;

    void Start()
    {
        _ediblesManager = FindFirstObjectByType<EdiblesManager>();
        _ediblesManager.DetectNextClosestItem();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ItemEater")
        {
            SandwhichSpawner sandwhichSpawner = FindFirstObjectByType<SandwhichSpawner>();
            sandwhichSpawner.EatItem(isBadItem);

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
