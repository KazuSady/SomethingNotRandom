using UnityEngine;

public class LiquidProvider : MonoBehaviour
{
    [SerializeField] private BoxCollider liquidSpillCollider;
    [SerializeField] private LiquidSpillChecker spillPoint;
    [SerializeField] private GameObject dropletPrefab;

    private void Awake()
    {
        spillPoint.OnDropSpill += HandleLiquidSpill;
    }

    private void Update()
    {
        var updatedRotation = liquidSpillCollider.transform.rotation;
        liquidSpillCollider.transform.rotation = new Quaternion(0.0f, updatedRotation.y, updatedRotation.z, updatedRotation.w);
    }
    
    private void OnDestroy()
    {
        spillPoint.OnDropSpill -= HandleLiquidSpill;
    }
    
    private void HandleLiquidSpill(Transform liquidDropStart)
    {
        var drop = Instantiate(dropletPrefab, liquidDropStart.position, Quaternion.identity);
        drop.GetComponentInChildren<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
    }
}
