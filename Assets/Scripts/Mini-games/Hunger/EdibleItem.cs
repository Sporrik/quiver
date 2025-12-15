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
                LaunchEdible();
                gameObject.tag = "Untagged";

                _ediblesManager.RemoveItemFromList(this);
                _ediblesManager.ClearInputIndex(this.InputIndex);
                _ediblesManager.DetectNextClosestItem();

                ControlSprite.gameObject.SetActive(false);

            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (other.name == "floor")
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(gameObject.tag == "Untagged") return;
        _ediblesManager.RemoveItemFromList(this);
        _ediblesManager.ClearInputIndex(this.InputIndex);
        _ediblesManager.DetectNextClosestItem();
    }

    public void DropEdible()
    {
        transform.SetParent(null);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
        _isCleared = true;
    }

    private void LaunchEdible()
    {
        transform.SetParent(null);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        rb.AddForce(Vector3.left * 3f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        _isCleared = true;
    }
}
