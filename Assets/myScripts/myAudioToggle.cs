using UnityEngine;
using UnityEngine.UI;

public class myAudioToggle : MonoBehaviour
{
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Sprite _soundOnSprite;
    [SerializeField] private Sprite _soundOffSprite;

    private bool _isMuted = false;

    private void Start()
    {
        AudioListener.pause = _isMuted;
        _updateButtonVisuals();
    }

    public void _toggleAudio()
    {
        _isMuted = !_isMuted;
        AudioListener.pause = _isMuted;
        _updateButtonVisuals();
    }

    private void _updateButtonVisuals()
    {
        if (_buttonImage == null) return;

        Sprite _targetSprite = _isMuted ? _soundOffSprite : _soundOnSprite;
        if (_targetSprite != null)
        {
            _buttonImage.sprite = _targetSprite;
        }

        Color _color = _buttonImage.color;
        _color.a = _isMuted ? 0.95f : 0.7f;
        _buttonImage.color = _color;
    }
}
