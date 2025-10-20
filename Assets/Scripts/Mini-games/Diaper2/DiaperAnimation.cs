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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetBool("isWorn", !_animator.GetBool("isWorn"));
        }
    }

    public void NewEvent()
    {
        //bruh
    }
}
