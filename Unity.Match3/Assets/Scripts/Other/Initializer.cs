using System.Collections;
using System.Collections.Generic;
using Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Initializer
{
    private static List<IInitializerItem> _items = new List<IInitializerItem>();

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async void EditorInit()
    {
        await Init();
    }
#endif

    public static async UniTask Init()
    {
        SetItems();

        foreach (IInitializerItem item in _items)
        {
            await item.Init();
        }
    }

    private static void SetItems()
    {
        _items.Clear();
        _items.Add(AudioController.Instance);
    }
}
