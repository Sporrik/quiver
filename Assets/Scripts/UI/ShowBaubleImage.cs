using UnityEngine;

public class ShowBaubleImage : MonoBehaviour
{
    [SerializeField] private TwitchGameManager twitchGameManager;
    [SerializeField] private GameObject bauble;
    void Start()
    {
        twitchGameManager = GameObject.FindGameObjectWithTag("Twitch").GetComponent<TwitchGameManager>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(twitchGameManager.GetBabyBrabbleMessage() == null || !twitchGameManager.GetBabyBrabbleMsgOnScreen())
    //    {
    //        bauble.SetActive(false);
    //    }
    //    else
    //    {
    //        bauble.SetActive(true);
    //        Debug.Log(twitchGameManager.GetBabyBrabbleMessage());
    //        Debug.Log(twitchGameManager.GetBabyBrabbleUser().displayname);
    //    }
    //}
}
