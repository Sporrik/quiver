using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EdibleItem : MonoBehaviour
{
    [SerializeField] private bool _isBadItem = false;
    [SerializeField] private bool _isCleared = false;
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
        if (other.name == "ItemEater" && _isCleared == false)
        {
            SandwhichSpawner sandwhichSpawner = FindFirstObjectByType<SandwhichSpawner>();
            sandwhichSpawner.EatItem(_isBadItem);

            if(_isBadItem)
            {
                _baby.GetComponent<IncorrectFood>().TriggerFlash();
            }

            Destroy(gameObject);
        }

        if (other.name == "floor")
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _ediblesManager.RemoveItemFromList(this);
        _ediblesManager.ClearInputIndex(this.InputIndex);
        _ediblesManager.DetectNextClosestItem();
    }

    public void DropEdible()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
        _isCleared = true;
    }
}
