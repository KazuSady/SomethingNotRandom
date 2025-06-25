using UnityEngine;

public class DecorBoxGameplay : MonoBehaviour
{
    [SerializeField] private Transform decorPlacement;
    [SerializeField] private DecorationGameplay decorationPrefab;
    [SerializeField] private DecorationGameplay decorationPrefabInstance;

    private void Awake()
    {
        decorationPrefabInstance.Grabbed += SpawnDecoration;
    }

    private void OnDestroy()
    {
        decorationPrefabInstance.Grabbed -= SpawnDecoration;
    }

    private void SpawnDecoration()
    {
        decorationPrefabInstance.Grabbed -= SpawnDecoration;
        decorationPrefabInstance = Instantiate(decorationPrefab, decorPlacement);
        decorationPrefabInstance.Grabbed += SpawnDecoration;
    }
    
}
