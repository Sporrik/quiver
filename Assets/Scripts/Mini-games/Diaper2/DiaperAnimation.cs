using UnityEngine;

public class DiaperAnimation : MonoBehaviour
{
    private Animator _animator;

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

            Debug.Log("poop!");
        }

        Debug.Log("pee!");
    }

    public void NewEvent()
    {
        //bruh
    }
}
