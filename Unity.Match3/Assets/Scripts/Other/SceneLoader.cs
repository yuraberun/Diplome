using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static async UniTask LoadScene(SceneEnum scene, LoadSceneMode loadMode, Action callback = null)
    {
        await (LoadScene(GetSceneName(scene), loadMode, callback));
    }

    public static async UniTask LoadScene(string sceneName, LoadSceneMode loadMode, Action callback = null)
    {
        if (!AddressableController.Instance.IsSceneInAddressables(sceneName) || !AddressableController.Instance.IsSceneRemote(sceneName))
        {
            await SceneManager.LoadSceneAsync(sceneName, loadMode);
        }
        else
        {
            await AddressableController.Instance.LoadSceneAsync(sceneName, loadMode, true).Task;
        }
    }

    public static async UniTask UnloadScene(SceneEnum scene)
    {
        await UnloadScene(GetSceneName(scene));
    }

    public static async UniTask UnloadScene(string sceneName)
    {
        await SceneManager.UnloadSceneAsync(sceneName);
    }

    public static void SetActiveScene(SceneEnum scene)
    {
        string sceneName = GetSceneName(scene);

        SetActiveScene(sceneName);
    }

    public static string GetActiveScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    public static bool IsSceneLoaded(SceneEnum scene)
    {
        string sceneName = GetSceneName(scene);

        return IsSceneLoaded(sceneName);
    }

    public static bool IsSceneLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }

    public static string GetSceneName(SceneEnum scene) => scene switch
    {
        SceneEnum.Main => "MainScene",
        _ => ""
    };

    public static SceneEnum GetSceneEnum(string scene) => scene switch
    {
        _ => SceneEnum.Main
    };
}
