using UnityEngine;
using UnityEditor;

namespace SimpleUI
{
#if UNITY_EDITOR

    [ExecuteInEditMode]
    public class RectTransformSizeFitEditorSupport : MonoBehaviour
    {
        private void OnRectTransformDimensionsChange()
        {
            if (!Application.isEditor || Application.isPlaying) return;

            // D.Log ("RectTransformSizeFitTest OnRectTransformDimensionsChange");
            var rtsf = this.GetComponent<RectTransformSizeFit>();

            if (rtsf == null) return;
            if (rtsf._rt == null)
            {
                rtsf._rt = GetComponent<RectTransform>();
            }

            if (rtsf._parentRt == null)
            {
                rtsf._parentRt = rtsf._rt.parent.GetComponent<RectTransform>();
            }

            rtsf.OnRectTransformDimensionsChange();
        }

    }

    [CustomEditor(typeof(RectTransformSizeFit))]
    public class RectTransformSizeFitRefreshBtn : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (RectTransformSizeFit)target;
            if (GUILayout.Button("Refresh Rate"))
            {
                myScript.RefreshRate();
            }

            if (GUILayout.Button("Refresh Layout"))
            {
                myScript.OnRectTransformDimensionsChange();
            }
        }
    }
#endif
}