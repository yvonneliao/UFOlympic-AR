using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class myDelayedAudio : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _delay = 1.5f;

    private void Start()
    {
        AudioSource _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.spatialBlend = 0f;

        if (_clip != null)
        {
            _source.clip = _clip;
            _source.PlayDelayed(_delay);
        }
    }
}
