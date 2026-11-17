using System.Collections;
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

    private static bool _hasSavedPlacement;
    private static Vector3 _savedCamOffset = new Vector3(0f, -0.8f, 1.5f);
    private static float _savedYawDelta;

    private readonly List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private bool _barnSpawned;

    private void Start()
    {
        if (_scanPrompt != null) _scanPrompt.SetActive(!_hasSavedPlacement);
        if (_tapPrompt != null) _tapPrompt.SetActive(false);

        if (_hasSavedPlacement)
        {
            StartCoroutine(_autoPlaceOnRestart());
        }
    }

    private void Update()
    {
        if (_barnSpawned || _hasSavedPlacement) return;

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
                Transform _cam = Camera.main != null ? Camera.main.transform : null;
                if (_cam != null)
                {
                    Quaternion _flatCamRot = Quaternion.Euler(0f, _cam.eulerAngles.y, 0f);
                    _savedCamOffset = Quaternion.Inverse(_flatCamRot) * (_hitPose.position - _cam.position);
                    _savedYawDelta = Mathf.DeltaAngle(_cam.eulerAngles.y, _hitPose.rotation.eulerAngles.y);
                }
                _hasSavedPlacement = true;

                _spawnGameplay(_hitPose.position, _hitPose.rotation);
            }
        }
    }

    private IEnumerator _autoPlaceOnRestart()
    {
        float _elapsed = 0f;
        while (_elapsed < 0.5f)
        {
            if (ARSession.state == ARSessionState.SessionTracking && Camera.main != null)
            {
                Vector2 _center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                if (_arRaycastManager != null && _arRaycastManager.Raycast(_center, _hits, TrackableType.PlaneWithinPolygon))
                {
                    _spawnGameplay(_hits[0].pose.position, _hits[0].pose.rotation);
                    yield break;
                }
            }
            _elapsed += Time.deltaTime;
            yield return null;
        }

        Transform _cam = Camera.main != null ? Camera.main.transform : null;
        if (_cam != null)
        {
            Quaternion _flatCamRot = Quaternion.Euler(0f, _cam.eulerAngles.y, 0f);
            Vector3 _spawnPos = _cam.position + _flatCamRot * _savedCamOffset;
            Quaternion _spawnRot = Quaternion.Euler(0f, _cam.eulerAngles.y + _savedYawDelta, 0f);
            _spawnGameplay(_spawnPos, _spawnRot);
        }
        else
        {
            _spawnGameplay(Vector3.zero, Quaternion.identity);
        }
    }

    private void _spawnGameplay(Vector3 _position, Quaternion _rotation)
    {
        GameObject _spawnedGameplay = Instantiate(_gameplayPrefab, _position, _rotation);

        GameObject _ufo = GameObject.FindWithTag("Player");
        if (_ufo != null)
            Variables.ActiveScene.Set("_UFOManager", _ufo);

        GameObject _guardian = GameObject.FindWithTag("Guardian");
        if (_guardian != null)
            CustomEvent.Trigger(_guardian, "GameStart");

        NavMeshSurface _navSurface = _spawnedGameplay.GetComponent<NavMeshSurface>();
        if (_navSurface != null) _navSurface.BuildNavMesh();

        GameObject _gameManager = GameObject.Find("Game Manager");
        if (_gameManager != null)
        {
            int _initialPrey = GameObject.FindGameObjectsWithTag("Prey").Length;
            Variables.Object(_gameManager).Set("preyCount", _initialPrey);

            Transform _transition = _spawnedGameplay.transform.Find("MeshParent/Game Canvas/Transition");
            if (_transition != null)
            {
                CanvasGroup _transitionGroup = _transition.GetComponent<CanvasGroup>();
                if (_transitionGroup != null)
                {
                    _transitionGroup.alpha = 0f;
                    _transition.gameObject.SetActive(true);
                    Variables.Object(_gameManager).Set("transitionGroup", _transitionGroup);
                }
            }
        }

        if (_spawnAudio != null) _spawnAudio.Play();

        _barnSpawned = true;
        if (_scanPrompt != null) _scanPrompt.SetActive(false);
        if (_tapPrompt != null) _tapPrompt.SetActive(false);

        if (_arPlaneManager != null)
        {
            _arPlaneManager.SetTrackablesActive(false);
            _arPlaneManager.enabled = false;
        }
    }
}
