using System;
using UnityEngine;

public class MonsterGameplay : MonoBehaviour
{
    private MonsterHappiness _monsterHappiness = MonsterHappiness.Medium;
    private CoffeeSO _desiredCoffee;
    
    public CoffeeSO DesiredCoffee => _desiredCoffee;

    public void InitializeMonster(GameObject newLook, CoffeeSO newCoffee)
    {
        Instantiate(newLook, transform);
        _desiredCoffee = newCoffee;
    }

    public void ChangeHappiness(MonsterHappiness newHappiness)
    {
        _monsterHappiness = newHappiness;
    }
}

[Serializable]
public enum MonsterHappiness
{
    Angry,
    Medium,
    Happy
}
