using System;
using SimpleUI;
using UnityEngine;

[RequireComponent(typeof(ButtonEffect.ButtonSoundEffect)), DisallowMultipleComponent]
public class UILoaderBtn : MonoBehaviour
{
    [SerializeField] public String PanelName = "";
    [SerializeField] public String SimpleData = "";

    public void zzBtnClicked()
    {
        if (!string.IsNullOrEmpty(SimpleData))
            UILoader.Show<string>(PanelName, SimpleData);
        else
            UILoader.Show(PanelName);
    }
}
