using System.Collections;
using System.Collections.Generic;
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

    private void OnDestroy()
    {
        if (_mug != null)
        {
            _mug.CoffeeStateUpdated -= OnCoffeeUpdated;
        }
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

        _currentMonster.InitializeMonster(look, coffee);
    }

    private void DisplayOrder(CoffeeSO coffee)
    {
        canvas.SetActive(true);
        textBubble.text = $"I want {coffee.CoffeeName}! :>";
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
    
    private void OnCoffeeUpdated(MugGameplay _mug)
    {
        // TODO
    }

    public void OnMugPicked(MugGameplay pickedMug)
    {
        if (_mug != null && _mug != pickedMug)
        {
            RemoveMug(_mug);
        }

        _mug = pickedMug;
        _mug.CoffeeStateUpdated += OnCoffeeUpdated;
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
            _mug.CoffeeStateUpdated -= OnCoffeeUpdated;
            _mug = null;

           tutorialManager.RemoveOldMug();
            
        }

        Destroy(mug.gameObject);
    }

    private int EvaluateCoffee()
    {
        CoffeeSO coffeeOrder = _currentMonster.DesiredCoffee;
        bool correctCup = _mug.CupType == coffeeOrder.RequiredCup;
        bool coffeeOk = WithinTolerance(_mug.GetCoffeeAmount(), coffeeOrder.CoffeeAmount);
        bool milkOk = WithinTolerance(_mug.GetMilkAmount(), coffeeOrder.MilkAmount);
        bool waterOk = WithinTolerance(_mug.GetWaterAmount(), coffeeOrder.WaterAmount);
        bool cremeOk = WithinTolerance(_mug.GetCremeAmount(), coffeeOrder.CremeAmount);

        int satisfaction = 0;
        if (correctCup)
        {
            satisfaction += 1;
        }
        if (coffeeOk)
        {
            satisfaction += 1;
        }
        if (milkOk)
        {
            satisfaction += 1;
        }
        if (waterOk)
        {
            satisfaction += 1;
        }
        if (cremeOk)
        {
            satisfaction += 1;
        }

        return satisfaction;
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
