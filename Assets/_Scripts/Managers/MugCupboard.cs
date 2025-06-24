using UnityEngine;

public class MugCupboard : MonoBehaviour
{
    public static MugCupboard Instance;

    [Header("Mug Prefabs")]
    [SerializeField] private GameObject _smallMugPrefab;
    [SerializeField] private GameObject _normalMugPrefab;
    [SerializeField] private GameObject _tallMugPrefab;
    [SerializeField] private GameObject _bigMugPrefab;

    [Header("Mug Positions")]
    [SerializeField] private Transform _smallMugPosition;
    [SerializeField] private Transform _normalMugPosition;
    [SerializeField] private Transform _tallMugPosition;
    [SerializeField] private Transform _bigMugPosition;

    private GameObject _smallMugInstance;
    private GameObject _normalMugInstance;
    private GameObject _tallMugInstance;
    private GameObject _bigMugInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnSmallMug();
        SpawnNormalMug();
        SpawnTallMug();
        SpawnBigMug();
    }
    

    public void OnMugGrabbed(MugGameplay mug)
    {
        if (GameManager.Instance.HasActiveMug() && GameManager.Instance.CurrentMug != mug)
        {
            ReturnMugToCupboard(GameManager.Instance.CurrentMug);
        }

        GameManager.Instance.OnMugPicked(mug);
    }

    public void ReturnMugToCupboard(MugGameplay mug)
    {
        CupType mugType = mug.CupType;
        GameManager.Instance.RemoveMug(mug);
        switch (mugType)
        {
            case CupType.Small:
                SpawnSmallMug();
                break;
            case CupType.Normal:
                SpawnNormalMug();
                break;
            case CupType.Tall:
                SpawnTallMug();
                break;
            case CupType.Big:
                SpawnBigMug();
                break;
        } 
    }

    private void SpawnSmallMug()
    {
        _smallMugInstance = Instantiate(_smallMugPrefab, _smallMugPosition);
    }

    private void SpawnNormalMug()
    {
        _normalMugInstance = Instantiate(_normalMugPrefab, _normalMugPosition);
    }

    private void SpawnTallMug()
    {
        _tallMugInstance = Instantiate(_tallMugPrefab, _tallMugPosition);
    }

    private void SpawnBigMug()
    {
        _bigMugInstance = Instantiate(_bigMugPrefab, _bigMugPosition);
    }
}
