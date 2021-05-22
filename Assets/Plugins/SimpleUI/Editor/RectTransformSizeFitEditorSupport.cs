using UnityEngine;
using UnityEditor;

namespace SimpleUI
{
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
}