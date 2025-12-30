using UnityEngine;

public class ShowBaubleImage : MonoBehaviour
{
    [SerializeField] private TwitchGameManager _twitchGameManager;
    [SerializeField] private GameObject _bauble;
    void Start()
    {
        _bauble.SetActive(false);
        _twitchGameManager = GameObject.FindGameObjectWithTag("Twitch").GetComponent<TwitchGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_twitchGameManager.GetBabyBrabbleMessage() == null || !_twitchGameManager.GetBabyBrabbleMsgOnScreen())
        {
            _bauble.SetActive(false);
        }
        else
        {
            _bauble.SetActive(true);
            //Debug.Log(twitchGameManager.GetBabyBrabbleMessage());
            //Debug.Log(twitchGameManager.GetBabyBrabbleUser().displayname);
        }
    }
}
