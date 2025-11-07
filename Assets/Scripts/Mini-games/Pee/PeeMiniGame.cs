using UnityEngine;
using TMPro;

public class PeeMiniGame : MonoBehaviour
{
    public int CurrentPeeAmount = 0;
    public int TargetPeeAmount = 300;

    public Transform PeeOrigin;
    public Transform Baby;
    public TextMeshProUGUI PeeCountText;
    public ParticleSystem PeeEffect;
    public ParticleSystem SplashEffect;
    public int PeeRange = 30;
    public float Speed = 75f;

    private int _currentRange;
    private float _currentSpeed;

    private bool _turnLeft = false;
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
            _currentSpeed = Random.Range(10f, Speed);
            _interval = Random.Range(1f, 5f);

            // Access the Emission Module and modify rateOverTime
            var emission = PeeEffect.emission;
            emission.rateOverTime = Random.Range(25, 50);

            var splashEmission = SplashEffect.emission;
            splashEmission.rateOverTime = Random.Range(20, 40);

            _timer = 0f;
        }

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
            _currentRange = Random.Range(1, PeeRange);
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
            Debug.Log("Pee Mini-game Complete!");
        }
    }
}
