using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunScene : MonoBehaviour
{
    [SerializeField] public String SceneName = "";

    public void zzBtnClicked()
    {
        SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single).completed += (AsyncOperation op) =>
        {
            if (op.isDone)
            {
                op.allowSceneActivation = true;
            }
        };
    }
}
