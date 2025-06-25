using System;
using UnityEngine;

public class MonsterGameplay : MonoBehaviour
{
    [Header("Responses")]
    [SerializeField] private string[] angryResponses;
    [SerializeField] private string[] mediumResponses;
    [SerializeField] private string[] happyResponses;

    private MonsterHappiness _monsterHappiness = MonsterHappiness.Medium;
    private CoffeeSO _desiredCoffee;
    private DecorationType _desiredDecorationType;
    private float _desiredTemperature;
    private System.Random _random = new();
    
    public CoffeeSO DesiredCoffee => _desiredCoffee;
    public DecorationType DesiredDecorationType => _desiredDecorationType;
    public float DesiredTemperature => _desiredTemperature;

    public void InitializeMonster(GameObject newLook, CoffeeSO newCoffee,
        DecorationType newDecorationType, float desiredTemperature)
    {
        Instantiate(newLook, transform);
        _desiredCoffee = newCoffee;
        _desiredDecorationType = newDecorationType;
        _desiredTemperature = desiredTemperature;
    }

    public void ChangeHappiness(MonsterHappiness newHappiness)
    {
        _monsterHappiness = newHappiness;
    }

    public string GetHappinessText()
    {
        switch (_monsterHappiness)
        {
            case MonsterHappiness.Angry:
                return GetRandomResponse(angryResponses);
            case MonsterHappiness.Medium:
                return GetRandomResponse(mediumResponses);
            case MonsterHappiness.Happy:
                return GetRandomResponse(happyResponses);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetRandomResponse(string[] responses)
    {
        if (responses == null || responses.Length == 0)
        {
            return string.Empty;
        }

        return responses[_random.Next(responses.Length)];
    }
}

[Serializable]
public enum MonsterHappiness
{
    Angry,
    Medium,
    Happy
}
