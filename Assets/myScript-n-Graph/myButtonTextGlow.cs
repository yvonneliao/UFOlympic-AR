using TMPro;
using UnityEngine;

public class myButtonTextGlow : MonoBehaviour
{
    [Header("Fade Animation")]
    [SerializeField] private float _pulseSpeed = 2f;
    [SerializeField, Range(0f, 1f)] private float _minAlpha = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _maxAlpha = 1f;

    [Header("Outer Glow Animation")]
    [SerializeField] private bool _animateOuterGlow = true;
    [SerializeField, Range(0f, 1f)] private float _minGlowPower = 0.1f;
    [SerializeField, Range(0f, 1f)] private float _maxGlowPower = 0.8f;

    private TMP_Text[] _texts;
    private Color[] _baseColors;
    private Material _sharedMat;

    private void Awake()
    {
        _texts = GetComponentsInChildren<TMP_Text>(true);
        _baseColors = new Color[_texts.Length];
        for (int _i = 0; _i < _texts.Length; _i++)
            _baseColors[_i] = _texts[_i].color;

        if (_texts.Length > 0)
            _sharedMat = _texts[0].fontSharedMaterial;
    }

    private void Update()
    {
        float _t = (Mathf.Sin(Time.time * _pulseSpeed) + 1f) * 0.5f;
        float _alpha = Mathf.Lerp(_minAlpha, _maxAlpha, _t);

        for (int _i = 0; _i < _texts.Length; _i++)
        {
            Color _c = _baseColors[_i];
            _c.a = _alpha;
            _texts[_i].color = _c;
        }

        if (_animateOuterGlow && _sharedMat != null && _sharedMat.HasProperty(ShaderUtilities.ID_GlowPower))
        {
            float _glow = Mathf.Lerp(_minGlowPower, _maxGlowPower, _t);
            _sharedMat.SetFloat(ShaderUtilities.ID_GlowPower, _glow);
        }
    }
}
