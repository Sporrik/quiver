using Unity.Mathematics;
using UnityEngine;

public class PeeOffset : MonoBehaviour
{
    public Transform PeeOrigin;
    public Transform PeeTarget;
    public ParticleSystem piss;
    public Transform ParticleParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = 
            new Vector3
            (
                transform.position.x + transform.rotation.x,
                gameObject.transform.position.y,
                PeeTarget.transform.transform.position.z
            );
       
        piss.startSpeed = (PeeOrigin.transform.position - gameObject.transform.position).magnitude * 3.25f;
    }
}
