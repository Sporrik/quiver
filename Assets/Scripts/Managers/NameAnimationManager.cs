using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.StickyNote;

public class NameAnimationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created




    [Header("AnimationTwitch")]

    [SerializeField] private ParticleSystem _addNameEffectPoo;
    [SerializeField] private ParticleSystem _addNameEffectPee;
    [SerializeField] private ParticleSystem _addNameEffectHunger;



    [SerializeField] private float _progressTimerPee;
    [SerializeField] private float _progressTimerPoo;
    [SerializeField] private float _progressTimerHunger;

    [SerializeField] private Color colorStart;
    [SerializeField] private Color ColorEnd;

    [SerializeField] private float animationTime = 2f;



    [SerializeField] private TextMeshProUGUI _currentTwitchTextPoo;
    [SerializeField] private TextMeshProUGUI _currentTwitchTextPee;
    [SerializeField] private TextMeshProUGUI _currentTwitchTextHunger;

    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPositionPee;
    [SerializeField] private Transform _endPositionPoo;
    [SerializeField] private Transform _endPositionHunger;


    List<string> _pooNamesList = new List<string> { };
    List<string> _hungerNamesList = new List<string> { };
    List<string> _peeNamesList = new List<string> { };

    private const float NAMEDELAY = 0.5f;
    [SerializeField] private float _addNameDelay;

    [SerializeField] private TwitchGameManager _gameManager;

    private void Update()
    {
        GetNamesTwitch();
    }

    private void GetNamesTwitch()
    {
        // _gameManager.GetViewerCount();

        _addNameDelay += Time.deltaTime;

        AddToList(_gameManager.GetUserNameHungerCommand(), _hungerNamesList);
        AddToList(_gameManager.GetUserNamePeeCommand(), _peeNamesList);
        AddToList(_gameManager.GetUserNamePoopCommand(), _pooNamesList);

        _progressTimerPee = AnimateText(_endPositionPee.position, _peeNamesList, _progressTimerPee, _currentTwitchTextPee, _addNameEffectPee);
        _progressTimerPoo = AnimateText(_endPositionPoo.position, _pooNamesList, _progressTimerPoo, _currentTwitchTextPoo, _addNameEffectPoo);
        _progressTimerHunger = AnimateText(_endPositionHunger.position, _hungerNamesList, _progressTimerHunger, _currentTwitchTextHunger, _addNameEffectHunger);
    }

    private float AnimateText(Vector3 endPosition, List<string> list, float progressTimer, TextMeshProUGUI text, ParticleSystem particle)
    {
        if (list.Count != 0)
        {
            progressTimer += Time.deltaTime;
            float progress = progressTimer / animationTime;


            // Debug.Log("progress" + progress);

            text.text = list[0];   // set name


            text.color = Color.Lerp(colorStart, ColorEnd, progress);
            text.rectTransform.position = Vector3.Slerp(_startPosition.position, endPosition, progress); // move UI


            if (progressTimer > animationTime) // RESET UI
            {
                particle.Play();

                progressTimer = 0;
                list.RemoveAt(0);  // animation done
            }
            return progressTimer;
        }
        return 0;

    }

    private void AddToList(string name, List<string> list)
    {
        if (name != null && _addNameDelay > NAMEDELAY) // 0.5 delay to send name in the twitch script
        {
            _addNameDelay = 0;
            list.Add(name);
        }
    }
}
