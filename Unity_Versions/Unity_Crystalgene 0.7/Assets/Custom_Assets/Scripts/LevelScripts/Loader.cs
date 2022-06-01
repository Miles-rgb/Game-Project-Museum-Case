using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }
    public enum Scene 
    {
        Loading_Scene,
        Level_00,
        Level_01,
        Level_02,
        Level_03,
        Level_09,
    }

    private static Action onLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        //set a loader callback action to load the target scene
        onLoaderCallBack = () =>
        {
            GameObject LoadingGameObject = new GameObject("Loading Game Object");
            LoadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));            
        };
        //load the loading screen
        SceneManager.LoadScene(Scene.Loading_Scene.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    public static void LoaderCallBack()
    {
        //trigger after first update which lets the screen refresh
        //execute the loader callback action which will load the target scene
        if (onLoaderCallBack != null)
        {
            onLoaderCallBack();
            onLoaderCallBack = null;
        }
    }
}
