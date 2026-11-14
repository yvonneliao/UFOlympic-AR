using UnityEngine;

public class myPreySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _preyPrefab;
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 12;
    [SerializeField] private float _rangeX = 7f;
    [SerializeField] private float _rangeZ = 6f;

    private void OnEnable()
    {
        if (_preyPrefab == null) return;

        int _count = Random.Range(_minCount, _maxCount);
        for (int _i = 0; _i < _count; _i++)
        {
            float _randomX = Random.Range(-_rangeX, _rangeX);
            float _randomZ = Random.Range(-_rangeZ, _rangeZ);
            Vector3 _spawnPos = transform.TransformPoint(new Vector3(_randomX, 0f, _randomZ));

            Instantiate(_preyPrefab, _spawnPos, transform.rotation, transform);
        }
    }
}
