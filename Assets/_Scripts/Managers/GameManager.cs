using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private Material basicCoffeeMaterial;
    [SerializeField] private Material milkCoffeeMaterial;
     
    public Material BasicCoffeeMaterial => basicCoffeeMaterial;
    public Material MilkCoffeeMaterial => milkCoffeeMaterial;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}


