using System;
using UnityEngine;

public static class Logger
{
    public static void Log(object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        UnityEngine.Debug.Log(logMsg);
#endif
    }

    public static void LogWarning(object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        UnityEngine.Debug.LogWarning(logMsg);
#endif
    }

    public static void LogError(object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        UnityEngine.Debug.LogError(logMsg);
#endif
    }


    public static void Log(this object obj, object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        Log("[" + obj.GetType() + "] " + logMsg);
#endif
    }

    public static void LogWarning(this object obj, object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        LogWarning("[" + obj.GetType() + "] " + logMsg);
#endif
    }

    public static void LogError(this object obj, object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        LogError("[" + obj.GetType() + "] " + logMsg);
#endif
    }

    public static void Log(string type, object logMsg)
    {
#if !DISABLE_LOGS || UNITY_EDITOR
        Log("[" + type + "] " + logMsg);
#endif
    }


}
