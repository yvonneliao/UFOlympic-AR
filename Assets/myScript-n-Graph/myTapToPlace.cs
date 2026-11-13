using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class myTapToPlace : MonoBehaviour
{
    [SerializeField] private ARRaycastManager _arRaycastManager;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private GameObject _gameplayPrefab;
    [SerializeField] private AudioSource _spawnAudio;

    private readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private bool _barnSpawned;

    private void Update()
    {
        if (_barnSpawned || Touchscreen.current == null) return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 _touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            if (_arRaycastManager.Raycast(_touchPosition, _hits, TrackableType.PlaneWithinPolygon))
            {
                Pose _hitPose = _hits[0].pose;
                Instantiate(_gameplayPrefab, _hitPose.position, _hitPose.rotation);
                if (_spawnAudio != null) _spawnAudio.Play();

                _barnSpawned = true;
                _arPlaneManager.SetTrackablesActive(false);
                _arPlaneManager.enabled = false;
            }
        }
    }
}
