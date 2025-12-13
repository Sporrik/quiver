using UnityEngine;

public class PostInteractionScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject keycard;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ExecuteInteraction()
    {
        Debug.Log("Post Interaction Executed");

        Destroy(keycard);


        // destroy keycard
        // show next goal
        // change UI so its visible to player


    }
    public void InteractionFailed() 
    {
        Debug.Log("Post Interaction Failed");
    }

}
