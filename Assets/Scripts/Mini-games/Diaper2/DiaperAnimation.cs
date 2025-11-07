using UnityEngine;

public class DiaperAnimation : MonoBehaviour
{
    private Animator _animator;

    Vector2 _initialMousePosition;
    Vector2 _currentMousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            _animator.SetBool("frontIsWorn", !_animator.GetBool("frontIsWorn"));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _animator.SetBool("leftIsWorn", !_animator.GetBool("leftIsWorn"));
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _animator.SetBool("rightIsWorn", !_animator.GetBool("rightIsWorn"));
        }
    }

    public void NewEvent()
    {
        //bruh
    }
}
