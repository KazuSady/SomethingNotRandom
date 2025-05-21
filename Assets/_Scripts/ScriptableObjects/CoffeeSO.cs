using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeSO", menuName = "Scriptable Objects/CoffeeSO")]
public class CoffeeSO : ScriptableObject
{
    [SerializeField] private string coffeeName;
    [Header("In grams")]
    [SerializeField] private float coffeeAmount;
    [Header("In milliliters")]
    [SerializeField] private float milkAmount;
    [SerializeField] private float cremeAmount;
    [SerializeField] private float waterAmount;
    [SerializeField] private CupType requiredCup;
    
    public string CoffeeName => coffeeName;
    public float CoffeeAmount => coffeeAmount;
    public float MilkAmount => milkAmount;
    public float CremeAmount => cremeAmount;
    public float WaterAmount => waterAmount;
    public CupType RequiredCup => requiredCup;
}

[Serializable]
public enum CupType
{
    Small,
    Normal,
    Tall,
    Big
}
