using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterGameplay monsterPrefab;
    [SerializeField] private Transform monsterPlacement;
    [SerializeField] private List<GameObject> monsterLook;
    [SerializeField] private List<CoffeeSO> availableCoffees;
    [Header("UI Elements")]
    [SerializeField] private TMP_Text textBubble;
    [SerializeField] private GameObject canvas;
    
    private MonsterGameplay _currentMonster;

    private void Awake()
    {
        GetNewMonster();
    }

    private void GetNewMonster()
    {
        if (_currentMonster is not null)
        {
            Destroy(_currentMonster.gameObject);
        }
        _currentMonster = Instantiate(monsterPrefab, monsterPlacement);
        GenerateMonsterInfo();
    }

    private void GenerateMonsterInfo()
    {
        var randomIndex = Random.Range(0, availableCoffees.Count);
        var randomCoffee = availableCoffees[randomIndex];
        randomIndex = Random.Range(0, monsterLook.Count);
        var randomLook = monsterLook[randomIndex];
        _currentMonster.InitializeMonster(randomLook, randomCoffee);
        canvas.SetActive(true);
        textBubble.text = $"I want {randomCoffee.CoffeeName}! :>";
    }
}
