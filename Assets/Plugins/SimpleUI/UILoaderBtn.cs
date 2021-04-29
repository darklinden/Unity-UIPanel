using System;
using SimpleUI;
using UnityEngine;

public class UILoaderBtn : MonoBehaviour
{
    [SerializeField] public String PanelName = "";
    [SerializeField] public String SimpleData = "";

    public void zzBtnClicked()
    {
        UILoader.Show<string>(PanelName, SimpleData);
    }
}
