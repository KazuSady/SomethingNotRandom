using UnityEngine;

public class LiquidProvider : MonoBehaviour
{
    [SerializeField] private BoxCollider liquidSpillCollider;
    [SerializeField] private LiquidSpillChecker spillPoint;
    [SerializeField] private GameObject dropletPrefab;

    private float _originalXRotation;

    private void Awake()
    {
        if (spillPoint)
        {
            spillPoint.OnDropSpill += HandleLiquidSpill;
        }
        _originalXRotation = transform.rotation.x;
    }

    private void Update()
    {
        if (liquidSpillCollider == null)
        {
            return;
        }

        var updatedRotation = liquidSpillCollider.transform.rotation;
        liquidSpillCollider.transform.rotation = new Quaternion(_originalXRotation, updatedRotation.y, updatedRotation.z, updatedRotation.w);
    }

    private void OnDestroy()
    {
        if (spillPoint)
        {
            spillPoint.OnDropSpill -= HandleLiquidSpill;
        }
    }

    private void HandleLiquidSpill(Transform liquidDropStart)
    {
        var drop = Instantiate(dropletPrefab, liquidDropStart.position, Quaternion.identity);
        drop.GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
    }

    public void ForceSpawnDroplet()
    {
        HandleLiquidSpill(transform);
    }
}
