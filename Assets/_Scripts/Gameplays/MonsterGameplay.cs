using System;
using UnityEngine;

public class MonsterGameplay : MonoBehaviour
{
    private MonsterHappiness _monsterHappiness = MonsterHappiness.Medium;
    private CoffeeSO _desiredCoffee;
    private DecorationType _desiredDecorationType;
    private float _desiredTemperature;
    
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
                return "What the hell is even that!?";
            case MonsterHappiness.Medium:
                return "Is that all you got? Eh...";
            case MonsterHappiness.Happy:
                return "Yipee!";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

[Serializable]
public enum MonsterHappiness
{
    Angry,
    Medium,
    Happy
}
