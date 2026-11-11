using UnityEngine;
using TMPro;

public class myFpsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsText;
    private float _deltaTime;

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float _fps = 1.0f / _deltaTime;
        if (_fpsText != null)
        {
            _fpsText.text = $"{Mathf.CeilToInt(_fps)} FPS";
        }
    }
}
