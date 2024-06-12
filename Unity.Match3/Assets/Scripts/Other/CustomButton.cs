using System;
using Audio;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private OnPointerEvents _onPointerEvents;

    public bool Enabled { get => _onPointerEvents.isActive; set => _onPointerEvents.isActive = value; }

    private void Awake()
    {
        if (_playClickSound)
        {
            SubscribeToClick(PlayClickSound);
        }

        if (_scaleAnim)
        {
            _onPointerEvents.onPointerDown += (PointerEventData data) => DoScaleAnim(true);
            _onPointerEvents.onPointerUp += (PointerEventData data) => DoScaleAnim(false);
        }

        if (_colorAnim)
        {
            if (_colorImageTarget != null)
            {
                _initColor = _colorImageTarget.color;
            }
            else if (_colorTmpTarget != null)
            {
                _initColor = _colorTmpTarget.color;
            }

            _onPointerEvents.onPointerDown += (PointerEventData data) => ChangeColor(false);
            _onPointerEvents.onPointerUp += (PointerEventData data) => ChangeColor(true);
        }
    }

    public void SubscribeToClick(Action callBack)
    {
        _onPointerEvents.onPointerClick += (PointerEventData data) =>
        {
            callBack.Invoke();
        };
    }

    public void RemoveAllSubscribesFromClick()
    {
        _onPointerEvents.onPointerClick = null;
    }

    public void RemoveAllSubscribesFromClickAndAddClick()
    {
        _onPointerEvents.onPointerClick = null;
        SubscribeToClick(PlayClickSound);
    }

    #region Click Sound
    [SerializeField] private bool _playClickSound = true;
    [SerializeField, ShowIf("@this._playClickSound")] private ActionType _audioType = ActionType.UI_buttonClick1;

    public void PlayClickSound()
    {
        Sounds.Play(_audioType);
    }
    #endregion

    #region Scale Animation
    [SerializeField] private bool _scaleAnim = false;
    [SerializeField, ShowIf("@this._scaleAnim")] private Transform _scaleTarget = null;
    [SerializeField, ShowIf("@this._scaleAnim")] private float _scaleValue = 0.98f;
    [SerializeField, ShowIf("@this._scaleAnim")] private float _scaleTime = 0.1f;

    private Tween _scaleTween;

    private void DoScaleAnim(bool isPoiterDown)
    {
        Vector3 targetScale = Vector3.one * (isPoiterDown ? _scaleValue : 1);
        Vector3 oldScale = Vector3.one * (!isPoiterDown ? _scaleValue : 1);

        _scaleTween?.Kill();
        _scaleTarget.localScale = oldScale;
        _scaleTween = _scaleTarget.DOScale(targetScale, _scaleTime).SetUpdate(true);
    }

    public Transform GetScaleTarget()
    {
        return _scaleTarget;
    }

    public void ChangeScaleTarget(Transform newTarget)
    {
        if (_scaleTarget != null)
        {
            _scaleTween?.Kill();
            _scaleTarget.localScale = Vector3.one;
        }

        _scaleTarget = newTarget;
    }
    #endregion

    #region Color Animation
    [SerializeField] private bool _colorAnim = false;
    [SerializeField, ShowIf("@this._colorAnim")] private Image _colorImageTarget = null;
    [SerializeField, ShowIf("@this._colorAnim")] private TextMeshProUGUI _colorTmpTarget = null;
    [SerializeField, ShowIf("@this._colorAnim")] private Color _color = Color.white;

    private Color _initColor;

    private void ChangeColor(bool setDefault)
    {
        if (_colorImageTarget != null)
        {
            _colorImageTarget.color = (setDefault) ? _initColor : _color;
        }

        if (_colorTmpTarget != null)
        {
            _colorTmpTarget.color = (setDefault) ? _initColor : _color;
        }
    }

    public void ChangeFocusColor(Color color)
    {
        _color = color;
    }

    public void ChangeDefaultColor(Color color)
    {
        _initColor = color;
    }
    public void ChangeDefaultColorImmediate(Color color)
    {
        _initColor = color;
        ChangeColor(true);
    }
    #endregion

    [Title("Situational parameters")]
    public void SetAlpha(float value)
    {
        var color = _colorImageTarget.color;
        color.a = value;
        _colorImageTarget.color = color;
    }

    /// <summary>
    /// Set enabled status with default alpha values
    /// </summary>
    /// <param name="value"></param>
    public void SetEnabled(bool value)
    {
        var color = _colorImageTarget.color;
        color.a = value ? 1f : 0.4f;
        _colorImageTarget.color = color;
        Enabled = value;
    }

#if UNITY_EDITOR
    public void SimulateClick()
    {
        _onPointerEvents.onPointerClick?.Invoke(null);
    }
#endif

    private void OnDestroy()
    {
        _scaleTween?.Kill();
    }
}
