using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace SimpleUI
{
    [RequireComponent(typeof(Image)), DisallowMultipleComponent]
    public class CommonPanel : MonoBehaviour
    {
        [SerializeField] protected bool _autoAnimateShow = true;
        [SerializeField] protected bool _clickBgClose = true;
        [SerializeField] protected RectTransformSizeFit _animRectSizeFit = null;

        private static readonly Color BgColor = new Color(0, 0, 0, 150.0f / 255.0f);
        private Image _backGroundImage;

        public virtual void OnBtnCloseClicked()
        {
            if (_animRectSizeFit != null)
            {
                _animRectSizeFit.AnimDisable(() =>
                {
                    gameObject.SetActive(false);
                    _panelClosedCall?.Invoke();

                });
            }
            else
            {
                DelayDo(() =>
                {
                    gameObject.SetActive(false);
                    _panelClosedCall?.Invoke();
                });
            }
        }

        private Action _panelClosedCall = null;
        public void SetClosedCallback(Action callback)
        {
            _panelClosedCall = callback;
        }

        public virtual void SetData(System.Object data)
        {
            D.Warning("CommonPanel.setData No Implementation: ", this.name);
            D.Warning(data);
        }

        public virtual bool PreventAction
        {
            get
            {
                return true;
            }
        }

        protected virtual void OnEnable()
        {
            SetBg();

            var rt = GetComponent<RectTransform>();
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);

            if (this._animRectSizeFit != null)
            {
                _animRectSizeFit.BlockTouch();
            }

            if (this._autoAnimateShow && this._animRectSizeFit)
            {
                this._animRectSizeFit.AnimEnable(() =>
                {
                    this.OnShowOver();
                });
            }
        }

        protected virtual void OnShowOver()
        {

        }

        protected void DelayDo(float afterSec, Action sth)
        {
            StartCoroutine(DoSomethingAfterDelay(afterSec, () =>
            {
                sth?.Invoke();
            }));
        }

        protected void DelayDo(Action sth)
        {
            StartCoroutine(DoSomethingAfterDelay(0, () =>
            {
                sth?.Invoke();
            }));
        }

        private IEnumerator DoSomethingAfterDelay(float afterSec, Action sth)
        {
            yield return new WaitForSeconds(afterSec);

            sth?.Invoke();
        }

        private void Reset()
        {
            SetBg();
        }

        private void Start()
        {
            SetBg();
        }

        private void SetBg()
        {
            if (_backGroundImage == null)
            {
                _backGroundImage = GetComponent<Image>();
            }

            _backGroundImage.sprite = null;
            _backGroundImage.rectTransform.offsetMin = new Vector2(0, 0);
            _backGroundImage.rectTransform.offsetMax = new Vector2(0, 0);
            _backGroundImage.rectTransform.localScale = new Vector3(1, 1, 1);
            _backGroundImage.color = BgColor;

            var btnClose = gameObject.GetComponent<UnityEngine.UI.Button>();
            if (btnClose == null) btnClose = gameObject.AddComponent<UnityEngine.UI.Button>();
            btnClose.transition = UnityEngine.UI.Selectable.Transition.None;
            btnClose.onClick.AddListener(OnBgClicked);
        }

        private void OnBgClicked()
        {
            if (_clickBgClose) OnBtnCloseClicked();
        }
    }
}