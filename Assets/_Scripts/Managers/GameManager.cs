using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField, Range(0f, 1f)] private float ingredientTolerance = 0.1f;
    
    [SerializeField] private List<CoffeeSO> availableCoffees;

    [SerializeField] private MugGameplay mug;

    [Header("Monsters")]
    [SerializeField] private MonsterGameplay monsterPrefab;
    [SerializeField] private Transform monsterPlacement;
    [SerializeField] private List<GameObject> monsterLook;

    [Header("Materials")]   
    [SerializeField] private Material basicCoffeeMaterial;
    [SerializeField] private Material milkCoffeeMaterial;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text textBubble;
    [SerializeField] private GameObject canvas;
    
    private MonsterGameplay _currentMonster;
    public MonsterGameplay CurrentMonster => _currentMonster;

    public Material BasicCoffeeMaterial => basicCoffeeMaterial;
    public Material MilkCoffeeMaterial => milkCoffeeMaterial;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mug.CoffeeStateUpdated += OnCoffeeUpdated;
        SpawnNewMonster();
    }

    private void OnDestroy()
    {
        mug.CoffeeStateUpdated -= OnCoffeeUpdated;
    }


    public void SpawnNewMonster()
    {
        if (_currentMonster != null)
        {
            Destroy(_currentMonster.gameObject);
        }
        _currentMonster = Instantiate(monsterPrefab, monsterPlacement);
        GenerateMonsterInfo();
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

    public void SubmitCoffee()
    {
        if (_currentMonster == null)
            return;

        int satisfaction = EvaluateCoffee();
        StartCoroutine(ShowFeedbackAndGenerateNewMonster(satisfaction));
    }
    
    private void OnCoffeeUpdated(MugGameplay _mug)
    {
        // TODO
    }

    private int EvaluateCoffee()
    {
        CoffeeSO coffeeOrder = _currentMonster.DesiredCoffee;
        bool correctCup = mug.CupType == coffeeOrder.RequiredCup;
        bool coffeeOk = WithinTolerance(mug.GetCoffeeAmount(), coffeeOrder.CoffeeAmount);
        bool milkOk = WithinTolerance(mug.GetMilkAmount(), coffeeOrder.MilkAmount);
        bool waterOk = WithinTolerance(mug.GetWaterAmount(), coffeeOrder.WaterAmount);
        bool cremeOk = WithinTolerance(mug.GetCremeAmount(), coffeeOrder.CremeAmount);

        int satisfaction = 0;
        if (correctCup) satisfaction += 1;
        if (coffeeOk) satisfaction += 1;
        if (milkOk) satisfaction += 1;
        if (waterOk) satisfaction += 1;
        if (cremeOk) satisfaction += 1;

        return satisfaction;
    }

    private bool WithinTolerance(float current, float required)
    {
        if (required == 0f)
            return current <= 0.001f;

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
        SpawnNewMonster();
    }
    
    private IEnumerator ShowFeedback(int satisfaction)
    {
        MonsterHappiness mood = GetMood(satisfaction);

        _currentMonster.ChangeHappiness(mood);
        Debug.Log($"Client is {mood.ToString().ToLower()}!");

        yield return new WaitForSeconds(2f);
    }
}
