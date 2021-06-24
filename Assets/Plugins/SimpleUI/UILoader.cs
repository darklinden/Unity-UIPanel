using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace SimpleUI
{
    public sealed class UILoader
    {
        // --- Singleton ---
        private static readonly Lazy<UILoader> lazy = new Lazy<UILoader>(() => new UILoader());
        private static UILoader Instance { get { return lazy.Value; } }
        // --- Singleton ---

        private const string PanelPath = "Assets/UIPanel/";
        private string _scene = null;
        private string scene
        {
            get
            {
                if (_scene == null)
                {
                    _scene = SceneManager.GetActiveScene().name;
                }
                return _scene;
            }

            set
            {
                _scene = value;
            }
        }

        private Dictionary<string, Dictionary<string, GameObject>> _panelsOnCurrentScene;
        private Dictionary<string, Boolean> _panelsPreventAction;

        private UILoader()
        {
#if UNITY_EDITOR
            D.Log(GetType().Name, "constructor");
#endif

            SceneManager.activeSceneChanged += ChangedActiveScene;
            _panelsOnCurrentScene = new Dictionary<string, Dictionary<string, GameObject>>();
            _panelsPreventAction = new Dictionary<string, bool>();
        }

        private void ChangedActiveScene(Scene current, Scene next)
        {
            D.Log("ChangedActiveScene: ",
            current != null ? "current:" + current.name : "no current name",
            next != null ? "next:" + next.name : "no next name");

            scene = next.name;

            _panelsPreventAction.Clear();
            foreach (var k in _panelsOnCurrentScene.Keys)
            {
                if (k == scene) continue;

                if (!_panelsOnCurrentScene.ContainsKey(k)) continue;

                if (_panelsOnCurrentScene[k] != null && _panelsOnCurrentScene[k].Keys.Count > 0)
                {
                    foreach (var j in _panelsOnCurrentScene[k].Keys)
                    {
                        if (_panelsOnCurrentScene[k][j] != null) Addressables.ReleaseInstance(_panelsOnCurrentScene[k][j]);
                    }
                    _panelsOnCurrentScene[k].Clear();
                }
            }

            Resources.UnloadUnusedAssets();
        }

        private void SetPanel(string path, GameObject panel)
        {

            if (!_panelsOnCurrentScene.ContainsKey(scene))
            {
                _panelsOnCurrentScene[scene] = new Dictionary<string, GameObject>();
            }

            _panelsOnCurrentScene[scene][path] = panel;
        }

        private GameObject GetPanel(string path)
        {
            if (!_panelsOnCurrentScene.ContainsKey(scene))
            {
                _panelsOnCurrentScene[scene] = new Dictionary<string, GameObject>();
            }

            if (_panelsOnCurrentScene[scene].ContainsKey(path))
            {
                return _panelsOnCurrentScene[scene][path];
            }

            return null;
        }

        private bool _HasShownPanel()
        {
            bool has = false;
            foreach (var k in _panelsPreventAction.Keys)
            {
                if (_panelsPreventAction[k])
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public static bool HasShownPanel()
        {
            return Instance._HasShownPanel();
        }

        private void _MarkPanelShow(string key)
        {
            _panelsPreventAction[key] = true;
        }

        public static void MarkPanelPreventAction(string key)
        {
            Instance._MarkPanelShow(key);
        }

        private void _MarkPanelClosed(string key)
        {
            _panelsPreventAction[key] = false;
            _panelsPreventAction.Remove(key);
        }

        public static void MarkPanelWontPreventAction(string key)
        {
            Instance._MarkPanelClosed(key);
        }

        private void InstantiatePrefab(string path, Action<GameObject> loaded)
        {
            Addressables.InstantiateAsync(path).Completed += (AsyncOperationHandle<GameObject> obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    loaded?.Invoke(obj.Result);
                }
                else
                {
                    loaded?.Invoke(null);
                }
            };
        }

        public static void Show(string PanelName)
        {
            Instance._Show<object>(PanelName, null);
        }

        public static void Show<DataType>(string PanelName, DataType data) where DataType : class
        {
            Instance._Show<DataType>(PanelName, data);
        }

        private void _Show<DataType>(string PanelName, DataType data) where DataType : class
        {
            string path = System.IO.Path.Combine(PanelPath, PanelName + ".prefab");

            Type PanelType = Type.GetType(PanelName, false, true);
            if (PanelType != null)
            {
                Assert.IsTrue(PanelType.IsSubclassOf(typeof(CommonPanel)));
            }

            var p = GetPanel(path);
            if (p != null)
            {
                bool preventAction = true;
                CommonPanel c = null;
                if (PanelType != null) c = (CommonPanel)p.GetComponent(PanelType);
                if (c == null) c = p.GetComponent<CommonPanel>();
                if (c != null)
                {
                    try
                    {
                        c.SetClosedCallback(() =>
                        {
                            MarkPanelWontPreventAction(path);
                        });

                        if (data != null) c.SetData((System.Object)data);
                        preventAction = c.PreventAction;
                    }
                    catch (Exception e)
                    {
                        D.Log(e);
                    }
                }

                p.SetActive(true);

                // Bring Node To Front
                RectTransform trans = p.GetComponent<RectTransform>();
                if (trans != null) trans.SetAsLastSibling();

                if (preventAction) MarkPanelPreventAction(path);

                return;
            }

            // WaitView.Show();

            InstantiatePrefab(path, (GameObject panel) =>
            {
                Assert.IsNotNull(panel);

                var rt = panel.transform as RectTransform;
                if (rt != null)
                {
                    SetPanel(path, rt.gameObject);

                    bool preventAction = true;
                    CommonPanel c = null;
                    if (PanelType != null) c = (CommonPanel)panel.GetComponent(PanelType);
                    if (c == null) c = panel.GetComponent<CommonPanel>();
                    if (c != null)
                    {
                        try
                        {
                            c.SetClosedCallback(() =>
                            {
                                MarkPanelWontPreventAction(path);
                            });

                            if (data != null) c.SetData((System.Object)data);
                            preventAction = c.PreventAction;
                        }
                        catch (Exception e)
                        {
                            D.Log(e);
                        }
                    }

                    rt.gameObject.SetActive(false);

                    var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                    rt.SetParent(canvas.transform, true);
                    rt.SetAsLastSibling();

                    rt.gameObject.SetActive(true);

                    if (preventAction) MarkPanelPreventAction(path);
                }
            });
        }

        public static void Hide(string PanelName)
        {
            Instance._Hide(PanelName);
        }

        private void _Hide(string PanelName)
        {
            string path = System.IO.Path.Combine(PanelPath, PanelName + ".prefab");

            var p = GetPanel(path);
            if (p != null && p.activeSelf)
            {
                MarkPanelWontPreventAction(path);

                CommonPanel c = p.GetComponent<CommonPanel>();
                if (c != null)
                {
                    try
                    {
                        c.OnBtnCloseClicked();
                    }
                    catch (Exception e)
                    {
                        D.Log(e);
                    }
                }
            }
        }

        public static void HideAll()
        {
            Instance._HideAll();
        }

        private void _HideAll()
        {
            _panelsPreventAction.Clear();

            foreach (var kv in _panelsOnCurrentScene)
            {
                foreach (var subKv in kv.Value)
                {
                    GameObject p = subKv.Value;
                    if (p && p.activeSelf)
                    {
                        CommonPanel c = p.GetComponent<CommonPanel>();
                        if (c != null)
                        {
                            try
                            {
                                c.OnBtnCloseClicked();
                            }
                            catch (Exception e)
                            {
                                D.Log(e);
                            }
                        }
                    }
                }
            }
        }
    }
}