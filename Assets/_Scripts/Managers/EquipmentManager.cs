using AYellowpaper.SerializedCollections;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<GameObject, Transform> equipmentWithPlaces;
    [SerializeField] private SerializedDictionary<Collider, GameObject> equipmentWithColliders;
    private void OnTriggerEnter(Collider other)
    {
        if (equipmentWithColliders.ContainsKey(other))
        {
            var parent = equipmentWithColliders[other];
            parent.transform.position = equipmentWithPlaces[parent].position;
        }
    }
}
