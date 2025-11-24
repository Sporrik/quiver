using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EdiblesManager : MonoBehaviour
{
    [SerializeField] GameObject _baby;
    [SerializeField] private List<EdibleItem> _closestItems = new List<EdibleItem>(4);
    public List<InputAction> PlayerActions = new List<InputAction>(4);
    private PlayerInput _playerInput;
    private bool _usingController = false;

    private EdibleItem _index1;
    private EdibleItem _index2;
    private EdibleItem _index3;
    private EdibleItem _index4;

    [SerializeField] Sprite _cross;
    [SerializeField] Sprite _triangle;
    [SerializeField] Sprite _circle;
    [SerializeField] Sprite _square;

    [SerializeField] Sprite _right;
    [SerializeField] Sprite _left;
    [SerializeField] Sprite _up;
    [SerializeField] Sprite _down;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = FindFirstObjectByType<PlayerInput>();

        PlayerActions.Add(_playerInput.actions["Discard1"]);
        PlayerActions.Add(_playerInput.actions["Discard2"]);
        PlayerActions.Add(_playerInput.actions["Discard3"]);
        PlayerActions.Add(_playerInput.actions["Discard4"]);
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput != null)
        {
            string currentScheme = _playerInput.currentControlScheme;

            if (currentScheme == "Keyboard&Mouse")
            {
                _usingController = false;
            }
            else if (currentScheme == "Gamepad")
            {
                _usingController = true;
            }
            else
            {
                Debug.Log($"User is using an unknown control scheme: {currentScheme}");
            }
        }
    }

    public void EatItem(EdibleItem item)
    {
        Destroy(item.gameObject);
    }

    public void DetectNextClosestItem()
    {
        if (_closestItems.Count >= 4)
            return;

        EdibleItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("EdibleItem"))
        {
            // Calculate distance to baby
            float distance = Vector3.Distance(_baby.transform.position, item.transform.position);

            EdibleItem edibleItem = item.GetComponent<EdibleItem>();

            // Check if the item is valid and not already in the list
            if (edibleItem != null && distance < closestDistance && !_closestItems.Contains(edibleItem))
            {
                closestDistance = distance;
                closestItem = edibleItem;
            }
        }

        // Add the closest item only if it is not null
        if (closestItem != null)
        {
            _closestItems.Add(closestItem);
            ChooseInputKey(closestItem);
        }
        else
        {
            Debug.LogWarning("No available EdibleItem to add to the closest items list.");
        }
    }

    public void RemoveItemFromList(EdibleItem itemToRemove)
    {
        _closestItems.Remove(itemToRemove);
    }

    public void ChooseInputKey(EdibleItem item)
    {
        if (_index1 == null)
        {
            item.InputIndex = 1;
            _index1 = item;

            if (_usingController)
                item.ControlSprite.sprite = _triangle;
            if (!_usingController)
                item.ControlSprite.sprite = _up;
        }
        else if (_index2 == null)
        {
            item.InputIndex = 2;
            _index2 = item;

            if (_usingController)
                item.ControlSprite.sprite = _square;
            if (!_usingController)
                item.ControlSprite.sprite = _right;
        }
        else if (_index3 == null)
        {
            item.InputIndex = 3;
            _index3 = item;

            if (_usingController)
                item.ControlSprite.sprite = _cross;
            if (!_usingController)
                item.ControlSprite.sprite = _down;
        }
        else if (_index4 == null)
        {
            item.InputIndex = 4;
            _index4 = item;

            if (_usingController)
                item.ControlSprite.sprite = _circle;
            if (!_usingController)
                item.ControlSprite.sprite = _left;
        }
        else
            return;
    }
    public void ClearInputIndex(int index)
    {
        if(index == 1)
        {
            _index1 = null;
        }
        else if(index == 2)
        {
            _index2 = null;
        }
        else if(index == 3)
        {
            _index3 = null;
        }
        else if(index == 4)
        {
            _index4 = null;
        }
    }
    public void Discard1(InputAction.CallbackContext ctx)
    {
        if(_index1 != null)
        {
            if (ctx.canceled)
                Destroy(_index1.gameObject);
        }
    }
    public void Discard2(InputAction.CallbackContext ctx)
    {
        if(_index2 != null)
        {
            if (ctx.canceled)
                Destroy(_index2.gameObject);
        }
    }
    public void Discard3(InputAction.CallbackContext ctx)
    {
        if(_index3 != null)
        {
            if (ctx.canceled)
                Destroy(_index3.gameObject);
        }
    }
    public void Discard4(InputAction.CallbackContext ctx)
    {
        if(_index4 != null)
        {
            if (ctx.canceled)
                Destroy(_index4.gameObject);
        }
    }

}
