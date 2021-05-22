using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunScene : MonoBehaviour
{
    [SerializeField] public String SceneName = "";

    public void zzBtnClicked()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
