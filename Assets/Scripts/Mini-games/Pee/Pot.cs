using UnityEngine;
using System.Collections.Generic;

public class Pot : MonoBehaviour
{
    private PeeMiniGame _peeMiniGame;
    void Start()
    {
        _peeMiniGame = FindFirstObjectByType<PeeMiniGame>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name.Contains("Piss"))
        {
            _peeMiniGame.CurrentPeeAmount++;
        }
    }
}
