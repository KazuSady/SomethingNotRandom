using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField, Range(0f, 1f)] private float ingredientTolerance = 0.1f;
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private List<CoffeeSO> availableCoffees;
    [SerializeField] private MugCupboard cupboard;
    [SerializeField] private List<DecorationType> availableDecorations;
    [SerializeField] private List<float> availableTemperatures;

    [Header("Monsters")]
    [SerializeField] private MonsterGameplay monsterPrefab;
    [SerializeField] private Transform monsterPlacement;
    [SerializeField] private List<GameObject> monsterLook;
    [SerializeField] private float disappearX = -4.0f;
    [SerializeField] private float showX = -2.0f;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem happyParticles;
    [SerializeField] private ParticleSystem angryParticles;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text textBubble;
    [SerializeField] private GameObject canvas;
    
    private MonsterGameplay _currentMonster;
    public MonsterGameplay CurrentMonster => _currentMonster;

    private MugGameplay _mug;
    public MugGameplay CurrentMug => _mug;
    public bool HasActiveMug() => _mug != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnNewMonster());
    }
    
    private IEnumerator SpawnNewMonster()
    {
        canvas.SetActive(false);
        if (_currentMonster)
        {
            var tween = _currentMonster.transform.DOLocalMoveX(disappearX, 2.0f);
            yield return tween.WaitForCompletion();
            Destroy(_currentMonster.gameObject);
        }
        _currentMonster = Instantiate(monsterPrefab, monsterPlacement);
        GenerateMonsterInfo();
        var tween2 = _currentMonster.transform.DOLocalMoveX(showX, 2.0f);
        yield return tween2.WaitForCompletion();
        DisplayOrder(_currentMonster.DesiredCoffee);
    }
    
    private void GenerateMonsterInfo()
    {
        int randomIndex = Random.Range(0, availableCoffees.Count);
        CoffeeSO coffee = availableCoffees[randomIndex];

        randomIndex = Random.Range(0, monsterLook.Count);
        GameObject look = monsterLook[randomIndex];
        
        randomIndex = Random.Range(0, availableDecorations.Count);
        var decorType = (DecorationType)randomIndex;
        randomIndex = Random.Range(0, availableTemperatures.Count);
        var temperature = availableTemperatures[randomIndex];

        _currentMonster.InitializeMonster(look, coffee, decorType, temperature);
    }

    private void DisplayOrder(CoffeeSO coffee)
    {
        canvas.SetActive(true);
        var temperature = "";
        switch (_currentMonster.DesiredTemperature)
        {
            case 0.0f:
                temperature = "cold";
                break;
            case 50.0f:
                temperature = "warm";
                break;
            case 90.0f:
                temperature = "extra hot";
                break;
        }
        StringBuilder sb = new();

        sb.AppendLine($"I want {coffee.CoffeeName}!");
        sb.AppendLine($"Make it {temperature}!");

        if (_currentMonster.DesiredDecorationType == DecorationType.None)
        {
            sb.AppendLine("I don't want any decorations!");
        }
        else
        {
            sb.AppendLine($"Decorate it with {_currentMonster.DesiredDecorationType}!");
        }

        textBubble.text = sb.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.GetComponent<MugGameplay>() != null)
        {
            SubmitCoffee();
        }
    }

    private void SubmitCoffee()
    {
        if (_currentMonster == null)
        {
            return;
        }

        int satisfaction = EvaluateCoffee();
        StartCoroutine(ShowFeedbackAndGenerateNewMonster(satisfaction));
        cupboard.ReturnMugToCupboard(_mug);
    }
    
    public void OnMugPicked(MugGameplay pickedMug)
    {
        if (_mug != null && _mug != pickedMug)
        {
            RemoveMug(_mug);
        }

        _mug = pickedMug;
        tutorialManager.SetMug(_mug, _mug.GetComponentInChildren<Renderer>());
    }

    public void RemoveMug(MugGameplay mug)
    {
        if (_mug == null)
        {
            return;
        }
        
        if (_mug == mug)
        {
            _mug = null;

           tutorialManager.RemoveOldMug();
            
        }

        Destroy(mug.gameObject);
    }

    private int EvaluateCoffee()
    {
        CoffeeSO coffeeOrder = _currentMonster.DesiredCoffee;
        var desiredTemperature = _currentMonster.DesiredTemperature;
        var desiredDecor = _currentMonster.DesiredDecorationType;
        
        bool correctCup = _mug.CupType == coffeeOrder.RequiredCup;
        bool coffeeOk = WithinTolerance(_mug.GetCoffeeAmount(), coffeeOrder.CoffeeAmount);
        bool milkOk = WithinTolerance(_mug.GetMilkAmount(), coffeeOrder.MilkAmount);
        bool waterOk = WithinTolerance(_mug.GetWaterAmount(), coffeeOrder.WaterAmount);
        bool cremeOk = WithinTolerance(_mug.GetCremeAmount(), coffeeOrder.CremeAmount);
        bool temperatureOk = IsTemperatureOkay(_mug.GetTemperature(), desiredTemperature);
        bool decorOk = _mug.DecorType() == desiredDecor;

        int satisfaction = 0;

        List<string> parts = new()
        {
            $"Cup: {(correctCup ? "OK" : "not ok")}",
            $"Coffee: {(coffeeOk ? "OK" : "not ok")}",
            $"Milk: {(milkOk ? "OK" : "not ok")}",
            $"Water: {(waterOk ? "OK" : "not ok")}",
            $"Creme: {(cremeOk ? "OK" : "not ok")}",
            $"Temperature: {(temperatureOk ? "OK" : "not ok")}",
            $"Decoration: {(decorOk ? "OK" : "not ok")}"
        };

        satisfaction += correctCup ? 1 : 0;
        satisfaction += coffeeOk ? 1 : 0;
        satisfaction += milkOk ? 1 : 0;
        satisfaction += waterOk ? 1 : 0;
        satisfaction += cremeOk ? 1 : 0;
        satisfaction += temperatureOk ? 1 : 0;
        satisfaction += decorOk ? 1 : 0;

        Debug.Log($"Coffee evaluated. Satisfaction: {satisfaction} ({string.Join("; ", parts)})");

        return satisfaction;
    }

    private bool IsTemperatureOkay(float current, float required)
    {
        if (required == 0.0f)
        {
            return current < 50.0f;
        }
        if (Mathf.Approximately(required, 50.0f))
        {
            return current < 90.0f;
        }
        if (Mathf.Approximately(required, 90.0f))
        {
            return current <= 100.0f;
        }

        return false;
    }

    private bool WithinTolerance(float current, float required)
    {
        if (required == 0f)
        {
            return current <= 0.001f;
        }

        float min = required * (1f - ingredientTolerance);
        float max = required * (1f + ingredientTolerance);
        return current >= min && current <= max;
    }

    private MonsterHappiness GetMood(int satisfaction)
    {
        return satisfaction switch
        {
            < 2 => MonsterHappiness.Angry,
            < 4 => MonsterHappiness.Medium,
            _ => MonsterHappiness.Happy
        };
    }

    private IEnumerator ShowFeedbackAndGenerateNewMonster(int satisfaction)
    {
        yield return ShowFeedback(satisfaction);
        StartCoroutine(SpawnNewMonster());
    }
    
    private IEnumerator ShowFeedback(int satisfaction)
    {
        MonsterHappiness mood = GetMood(satisfaction);

        _currentMonster.ChangeHappiness(mood);
        switch (mood)
        {
            case MonsterHappiness.Angry:
                _currentMonster.transform.DOShakePosition(2.0f, 0.5f);
                angryParticles.Play();
                break;
            case MonsterHappiness.Medium:
                break;
            case MonsterHappiness.Happy:
                _currentMonster.transform.DOLocalJump(_currentMonster.transform.localPosition, 0.1f, 5, 2.0f);
                happyParticles.Play();
                break;
        }

        textBubble.text = _currentMonster.GetHappinessText();

        yield return new WaitForSeconds(2f);
    }
}
