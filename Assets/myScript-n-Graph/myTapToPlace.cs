using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

public class myTapToPlace : MonoBehaviour
{
    [SerializeField] private ARRaycastManager _arRaycastManager;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private GameObject _gameplayPrefab;
    [SerializeField] private AudioSource _spawnAudio;
    [SerializeField] private GameObject _scanPrompt;
    [SerializeField] private GameObject _tapPrompt;

    private readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private bool _barnSpawned;

    private void Start()
    {
        if (_scanPrompt != null) _scanPrompt.SetActive(true);
        if (_tapPrompt != null) _tapPrompt.SetActive(false);
    }

    private void Update()
    {
        if (_barnSpawned) return;

        bool _hasPlanes = _arPlaneManager != null && _arPlaneManager.trackables.count > 0;
        if (_scanPrompt != null && _scanPrompt.activeSelf == _hasPlanes)
            _scanPrompt.SetActive(!_hasPlanes);
        if (_tapPrompt != null && _tapPrompt.activeSelf != _hasPlanes)
            _tapPrompt.SetActive(_hasPlanes);

        if (Touchscreen.current == null) return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 _touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            if (_arRaycastManager.Raycast(_touchPosition, _hits, TrackableType.PlaneWithinPolygon))
            {
                Pose _hitPose = _hits[0].pose;
                GameObject _spawnedGameplay = Instantiate(_gameplayPrefab, _hitPose.position, _hitPose.rotation);

                GameObject _ufo = GameObject.FindWithTag("Player");
                if (_ufo != null)
                    Variables.ActiveScene.Set("_UFOManager", _ufo);

                GameObject _guardian = GameObject.FindWithTag("Guardian");
                if (_guardian != null)
                    CustomEvent.Trigger(_guardian, "GameStart");

                NavMeshSurface _navSurface = _spawnedGameplay.GetComponent<NavMeshSurface>();
                if (_navSurface != null) _navSurface.BuildNavMesh();

                if (_spawnAudio != null) _spawnAudio.Play();

                _barnSpawned = true;
                if (_scanPrompt != null) _scanPrompt.SetActive(false);
                if (_tapPrompt != null) _tapPrompt.SetActive(false);

                _arPlaneManager.SetTrackablesActive(false);
                _arPlaneManager.enabled = false;
            }
        }
    }
}
