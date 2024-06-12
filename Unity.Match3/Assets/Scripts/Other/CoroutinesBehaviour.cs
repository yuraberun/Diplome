using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CoroutinesBehaviour : MonoBehaviour
{
    private static CoroutinesBehaviour s_instance;

    public static CoroutinesBehaviour Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject obj = new GameObject("_coroutinesBehaviour");
                CoroutinesBehaviour beh = obj.AddComponent<CoroutinesBehaviour>();
                s_instance = beh;
                DontDestroyOnLoad(obj);
            }
            return s_instance;
        }
    }

    public static Coroutine Start(IEnumerator enumerator)
    {
        return Instance.StartCoroutine(enumerator);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (Instance != null && coroutine != null)
        {
            Instance.StopCoroutine(coroutine);
        }
    }

    public static Coroutine DoActionAfterTime(float time, Action action)
    {
        return DoActionAfterTime(new WaitForSeconds(time), action);
    }

    public static Coroutine DoActionAfterTime(WaitForSeconds waitForSeconds, Action action)
    {
        return Instance.StartCoroutine(DoActionAfterTime_Cor(waitForSeconds, action));
    }

    public static Coroutine DoActionAfterTime(WaitForSecondsRealtime waitForSeconds, Action action)
    {
        return Instance.StartCoroutine(DoActionAfterTime_Cor(waitForSeconds, action));
    }

    public static Coroutine DoActionAfterFrame(int countFrame, Action action)
    {
        return Instance.StartCoroutine(DoActionAfterFrame_Cor(countFrame, action));
    }

    public static Coroutine DoActionAfterOneFrame(Action action)
    {
        return Instance.StartCoroutine(DoActionAfterFrame_Cor(1, action));
    }

    private static IEnumerator DoActionAfterFrame_Cor(int countFrame, Action action)
    {
        for (int i = 0; i < countFrame; i++)
        {
            yield return 0;
        }
        action?.Invoke();
    }

    private static IEnumerator DoActionAfterTime_Cor(WaitForSeconds waitForSeconds, Action action)
    {
        yield return waitForSeconds;
        action?.Invoke();
    }

    private static IEnumerator DoActionAfterTime_Cor(WaitForSecondsRealtime waitForSeconds, Action action)
    {
        yield return waitForSeconds;
        action?.Invoke();
    }

    public static void StopAll(Coroutine coroutine)
    {
        Instance.StopAllCoroutines();
    }

    private void OnDestroy()
    {
        Instance.StopAllCoroutines();
    }
}
