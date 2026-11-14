using TMPro;
using UnityEngine;

public class myTargetCountDisplay : MonoBehaviour
{
    [SerializeField] private Transform _preyGroup;
    private TMP_Text _targetText;

    private void Awake()
    {
        _targetText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_preyGroup != null && _targetText != null)
        {
            _targetText.text = $"TARGETS LEFT: {_preyGroup.childCount}";
        }
    }
}
