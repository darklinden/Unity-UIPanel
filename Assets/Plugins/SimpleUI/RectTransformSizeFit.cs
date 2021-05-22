using System;
using DigitalRuby.Tween;
using UnityEngine;

namespace SimpleUI
{
    public class RectTransformSizeFit : MonoBehaviour
    {
        private enum EnumFitType
        {
            eNone = 0,
            WidthLimit = 1,
            HeightLimit = 2
        }

        private enum EFitAction
        {
            eNone = 0,
            eFitOnEnable = 1
        }

        [SerializeField] private EFitAction _fitAction = EFitAction.eFitOnEnable;

        [SerializeField] private EnumFitType _fitType = EnumFitType.eNone;

        [SerializeField] private float _maxRateLimitInParent = 1;
        [SerializeField] private float _minRateLimitInParent = 0;

        [SerializeField] [ReadOnly] private float _widthRateOfParent = 0;
        [SerializeField] [ReadOnly] private float _heightRateOfParent = 0;

        [SerializeField] private float _ratioWH = 1;

        public RectTransform _parentRt;
        public RectTransform _rt;
        private bool _autoFit = false;

        public void RefreshRate()
        {
            _rt = GetComponent<RectTransform>();
            _parentRt = _rt.parent.GetComponent<RectTransform>();

            if (_rt == null || _parentRt == null)
            {
                _maxRateLimitInParent = 0;
                _maxRateLimitInParent = 0;
                _widthRateOfParent = 0;
                _heightRateOfParent = 0;
                return;
            }

            _rt.localScale = Vector3.one;

            _ratioWH = _rt.rect.width / _rt.rect.height;

            switch (_fitType)
            {
                case EnumFitType.WidthLimit:
                    {
                        var h = _rt.rect.height;
                        var ph = _parentRt.rect.height;
                        _heightRateOfParent = h / ph;

                        _widthRateOfParent = _rt.rect.width / _parentRt.rect.width;
                    }
                    break;
                case EnumFitType.HeightLimit:
                    {
                        var w = _rt.rect.width;
                        var pw = _parentRt.rect.width;
                        _widthRateOfParent = w / pw;

                        _heightRateOfParent = _rt.rect.height / _parentRt.rect.height;
                    }
                    break;
            }
        }

        // Use this for initialization
        private void Start()
        {
            _autoFit = _fitAction == EFitAction.eFitOnEnable;
            if (_rt == null)
            {
                _rt = GetComponent<RectTransform>();
            }

            if (_parentRt == null)
            {
                _parentRt = _rt.parent.GetComponent<RectTransform>();
            }

            _ratioWH = _rt.rect.width / _rt.rect.height;
            OnRectTransformDimensionsChange();
        }

        private Vector3 FitWidth()
        {
            if (_rt == null || _parentRt == null)
                return Vector3.one;

            var h = _parentRt.rect.height * _heightRateOfParent;
            var w = h * _ratioWH;

            if (w < _parentRt.rect.width * _minRateLimitInParent)
            {
                w = _parentRt.rect.width * _minRateLimitInParent;
                h = w / _ratioWH;
            }
            else if (w > _parentRt.rect.width * _maxRateLimitInParent)
            {
                w = _parentRt.rect.width * _maxRateLimitInParent;
                h = w / _ratioWH;
            }

            var s = _rt.localScale;
            s.x = w / _rt.rect.width;
            s.y = h / _rt.rect.height;

            return s;
        }

        private Vector3 FitHeight()
        {
            if (_rt == null || _parentRt == null)
                return Vector3.one;

            var w = _parentRt.rect.width * _widthRateOfParent;
            var h = w / _ratioWH;

            if (h < _parentRt.rect.height * _minRateLimitInParent)
            {
                h = _parentRt.rect.height * _minRateLimitInParent;
                w = h * _ratioWH;
            }
            else if (h > _parentRt.rect.height * _maxRateLimitInParent)
            {
                h = _parentRt.rect.height * _maxRateLimitInParent;
                w = h * _ratioWH;
            }

            var s = _rt.localScale;
            s.x = w / _rt.rect.width;
            s.y = h / _rt.rect.height;

            return s;
        }

        public void OnRectTransformDimensionsChange()
        {
            if (!_autoFit) return;

            switch (_fitType)
            {
                case EnumFitType.WidthLimit:
                    _rt.localScale = FitWidth();
                    _widthRateOfParent = _rt.rect.width * _rt.localScale.x / _parentRt.rect.width;
                    break;
                case EnumFitType.HeightLimit:
                    _rt.localScale = FitHeight();
                    _heightRateOfParent = _rt.rect.height * _rt.localScale.y / _parentRt.rect.height;
                    break;
            }
        }

        public Vector3 FittedScale()
        {
            Vector3 s;
            switch (_fitType)
            {
                case EnumFitType.WidthLimit:
                    s = FitWidth();
                    break;
                case EnumFitType.HeightLimit:
                    s = FitHeight();
                    break;
                default:
                    s = Vector3.one;
                    break;
            }

            return s;
        }

        public void BlockTouch()
        {
            var btnBlocker = gameObject.GetComponent<UnityEngine.UI.Button>();
            if (btnBlocker == null) btnBlocker = gameObject.AddComponent<UnityEngine.UI.Button>();
            btnBlocker.transition = UnityEngine.UI.Selectable.Transition.None;
        }

        public void AnimEnable(Action completion)
        {
            _autoFit = false;
            var des_scale = Vector3.one;
            switch (_fitType)
            {
                case EnumFitType.WidthLimit:
                    {
                        des_scale = FitWidth();
                    }
                    break;
                case EnumFitType.HeightLimit:
                    {
                        des_scale = FitHeight();
                    }
                    break;
                default:
                    break;
            }

            _rt.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            gameObject.Tween("ScaleRtEnable", new Vector3(0.01f, 0.01f, 0.01f), des_scale, 0.15f, TweenScaleFunctions.CubicEaseIn,
            (t) =>
            {
                gameObject.transform.localScale = t.CurrentValue;
            }, (t) =>
            {
                _autoFit = true;
                switch (_fitType)
                {
                    case EnumFitType.WidthLimit:
                        _rt.localScale = FitWidth();
                        _widthRateOfParent = _rt.rect.width * _rt.localScale.x / _parentRt.rect.width;
                        break;
                    case EnumFitType.HeightLimit:
                        _rt.localScale = FitHeight();
                        _heightRateOfParent = _rt.rect.height * _rt.localScale.y / _parentRt.rect.height;
                        break;
                }

                completion?.Invoke();
            });
        }

        public void AnimDisable(Action completion)
        {
            _autoFit = false;

            gameObject.Tween("ScaleRtDisable", _rt.localScale, new Vector3(0.01f, 0.01f, 0.01f), 0.15f, TweenScaleFunctions.CubicEaseIn, (t) =>
            {
                gameObject.transform.localScale = t.CurrentValue;
            }, (t) =>
            {
                completion?.Invoke();
            });
        }
    }
}