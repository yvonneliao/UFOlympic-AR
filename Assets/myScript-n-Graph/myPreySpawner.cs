using System.Collections.Generic;
using UnityEngine;

public class myPreySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _preyPrefab;
    [SerializeField] private int _minCount = 3;
    [SerializeField] private int _maxCount = 12;
    [SerializeField] private float _rangeX = 7f;
    [SerializeField] private float _rangeZ = 6f;

    [Header("Overlap Prevention (Local Units)")]
    [SerializeField] private float _minPreySpacing = 1.8f;
    [SerializeField] private float _obstacleCheckRadius = 0.9f;
    [SerializeField] private float _checkHeightOffset = 0.7f;
    [SerializeField] private int _maxAttemptsPerSheep = 25;
    [SerializeField] private LayerMask _obstacleLayers = ~0;

    private void OnEnable()
    {
        if (_preyPrefab == null) return;

        int _count = Random.Range(_minCount, _maxCount);
        List<Vector3> _spawnedLocalPositions = new List<Vector3>(_count);
        float _worldRadius = _obstacleCheckRadius * transform.lossyScale.x;

        for (int _i = 0; _i < _count; _i++)
        {
            for (int _attempt = 0; _attempt < _maxAttemptsPerSheep; _attempt++)
            {
                float _randomX = Random.Range(-_rangeX, _rangeX);
                float _randomZ = Random.Range(-_rangeZ, _rangeZ);
                Vector3 _localPos = new Vector3(_randomX, 0f, _randomZ);

                if (_isTooCloseToOtherPrey(_localPos, _spawnedLocalPositions))
                    continue;

                float _effectiveHeight = Mathf.Max(_checkHeightOffset, _obstacleCheckRadius + 0.25f);
                Vector3 _checkWorldPos = transform.TransformPoint(new Vector3(_randomX, _effectiveHeight, _randomZ));
                if (_hasObstacleAt(_checkWorldPos, _worldRadius))
                    continue;

                Vector3 _spawnPos = transform.TransformPoint(_localPos);
                Quaternion _randomRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Instantiate(_preyPrefab, _spawnPos, _randomRot, transform);
                _spawnedLocalPositions.Add(_localPos);
                break;
            }
        }
    }

    private bool _hasObstacleAt(Vector3 _checkWorldPos, float _worldRadius)
    {
        Collider[] _hits = Physics.OverlapSphere(_checkWorldPos, _worldRadius, _obstacleLayers, QueryTriggerInteraction.Ignore);
        for (int _i = 0; _i < _hits.Length; _i++)
        {
            if (_hits[_i].GetComponentInParent<UnityEngine.XR.ARFoundation.ARPlane>() != null)
                continue;
            return true;
        }
        return false;
    }

    private bool _isTooCloseToOtherPrey(Vector3 _candidate, List<Vector3> _existing)
    {
        float _minSqrDist = _minPreySpacing * _minPreySpacing;
        for (int _i = 0; _i < _existing.Count; _i++)
        {
            if ((_candidate - _existing[_i]).sqrMagnitude < _minSqrDist)
                return true;
        }
        return false;
    }
}
