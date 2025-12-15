using UnityEngine;
using TMPro;

public class PeeMiniGame : MonoBehaviour
{
    public int CurrentPeeAmount = 0;
    [SerializeField] private int TargetPeeAmount = 300;

    [SerializeField] private Transform PeeOrigin;
    [SerializeField] private Transform Baby;
    [SerializeField] private TextMeshProUGUI PeeCountText;
    [SerializeField] private ParticleSystem PeeEffect;
    [SerializeField] private ParticleSystem SplashEffect;
    [SerializeField] private int PeeRange = 30;
    [SerializeField] private float Speed = 75f;
    public bool TaskComplete = false;
    [Header("Win condition:")]
    [SerializeField] private MinigameWinToggle _winCondition = null;
    [SerializeField] private float _winTriggerDelay = 1f;
    [SerializeField] private GameObject _peeVisual;
    [SerializeField] private GameObject _confettiParticle;
    [SerializeField] private GameObject _confettiParticle1;
    [SerializeField] private GameObject _confettiParticle2;

    private int _currentRange;
    private float _currentSpeed;

    [SerializeField] private bool _turnLeft = false;
    private float _timer;
    private float _interval = 3f;
    
    private int _percentageComplete = 0;

    void Start()
    {
        _currentRange = PeeRange;
        _currentSpeed = Speed;
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _currentSpeed = Random.Range(15f, Speed);
            _interval = Random.Range(1f, 5f);

            // Access the Emission Module and modify rateOverTime
            var emission = PeeEffect.emission;
            emission.rateOverTime = Random.Range(25, 50);

            var splashEmission = SplashEffect.emission;
            splashEmission.rateOverTime = Random.Range(20, 40);

            _timer = 0f;
        }

        // Map _percentageComplete (0-100) to the range 1 to -0.54f
        float targetScaleY = Mathf.Lerp(0f, 1.5f, _percentageComplete / 100f);
        float currentScaleY = _peeVisual.transform.localScale.y;

        _peeVisual.transform.localScale = new Vector3(
            _peeVisual.transform.localScale.x,
            Mathf.Lerp(currentScaleY, targetScaleY, Time.deltaTime * 5f), // 5f is the lerp speed
            _peeVisual.transform.localScale.z
        );

        if (_turnLeft)
        {
            PeeOrigin.transform.Rotate(0, Time.deltaTime * Mathf.Max(_currentSpeed, 1f), 0);
        }
        else
        {
            PeeOrigin.transform.Rotate(0, -Time.deltaTime * Mathf.Max(_currentSpeed, 1f), 0);
        }

        float angle = Vector3.Angle(PeeOrigin.forward, Baby.forward);

        if (angle == 0f)
        {
            _currentRange = Random.Range(20, PeeRange);
        }

        if (angle > _currentRange)
        {
            _turnLeft = !_turnLeft;
        }

        _percentageComplete = (int)(((float)CurrentPeeAmount / (float)TargetPeeAmount) * 100f);
        _percentageComplete = Mathf.Clamp(_percentageComplete, 0, 100);
        PeeCountText.text = $"{_percentageComplete} %";

        if (CurrentPeeAmount >= TargetPeeAmount)
        {
            //Debug.Log("Pee Mini-game Complete!");
            TaskComplete = true;
            _confettiParticle.SetActive(true);
            _confettiParticle1.SetActive(true);
            _confettiParticle2.SetActive(true);
        }

        // needed in order to trigger the closing of the minigame
        if (TaskComplete)
        {
            if(_winTriggerDelay <= 0)
            {
                _winCondition.WinMinigame();
            }

            _winTriggerDelay -= Time.deltaTime;
        }
    }
}
