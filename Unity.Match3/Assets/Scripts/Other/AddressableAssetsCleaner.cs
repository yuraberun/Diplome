using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

public class AddressableAssetsCleaner
{
    private static AddressableAssetsCleaner s_cleaner;

    private CancellationTokenSource _cancellationTokenSource = new();
    private CancellationToken _cancellationToken;

    private float _percentComplete;
    private OperationStatus _status;
    private OperationState _state;
    private bool _isDone;
    private bool _isValid;
    private Task _task;

    private AddressableAssetsCleaner() { }

    public event Action<OperationState, AddressableAssetsCleaner> StateChanged;
    public event Action<AddressableAssetsCleaner> Completed;
    public event Action<AddressableAssetsCleaner> Failed;
    public event Action<AddressableAssetsCleaner> Canceled;
    public event Action<float> RemovalAsset;

    public enum OperationState
    {
        None,
        SearchUnusedAssets,
        RemovalUnusedAssets,
        Finished,
    }

    public enum OperationStatus
    {
        None,
        Succeeded,
        Failed,
        Canceled
    }

    public static AddressableAssetsCleaner Cleaner => s_cleaner;

    public float PercentComplete => _percentComplete;
    public OperationStatus Status => _status;
    public OperationState State => _state;
    public bool IsDone => _isDone;
    public bool IsValid => _isValid;
    public Task Task => _task;

    public static AddressableAssetsCleaner StartCleanUp()
    {
        if (s_cleaner is not { IsValid: true })
        {
            s_cleaner = new AddressableAssetsCleaner();
            s_cleaner.Init();
        }
        return s_cleaner;
    }

    public static void AbortCleanUp()
    {
        if (s_cleaner is not { IsValid: true }) return;

        s_cleaner.Abort();
    }

    public void Abort()
    {
        _cancellationTokenSource.Cancel();
    }

    private void Init()
    {
        _cancellationToken = _cancellationTokenSource.Token;
        _task = DeleteUnusedAssetsAsync();
    }

    private void OnCompleted()
    {
        Completed?.Invoke(this);
    }

    private void OnFailed()
    {
        Failed?.Invoke(this);
    }

    private void OnCanceled()
    {
        Canceled?.Invoke(this);
    }

    private void OnStateChanged(OperationState state)
    {
        StateChanged?.Invoke(state, this);
    }

    private void OnRemovalAsset(float percentProgress)
    {
        RemovalAsset?.Invoke(percentProgress);
    }

    private async Task DeleteUnusedAssetsAsync()
    {
        _isDone = false;
        _isValid = true;
        try
        {
            if (_cancellationToken.IsCancellationRequested) return;

            _state = OperationState.SearchUnusedAssets;
            OnStateChanged(OperationState.SearchUnusedAssets);
            var unusedGroups = await FindUnusedGroups(_cancellationToken);

            if (_cancellationToken.IsCancellationRequested) return;

            _state = OperationState.RemovalUnusedAssets;
            OnStateChanged(OperationState.RemovalUnusedAssets);
            await DeleteUnusedGroupsAsync(unusedGroups, _cancellationToken);

            _status = OperationStatus.Succeeded;
            OnCompleted();
        }
        catch (Exception e)
        {
            Log($"Failed to clear assets. {e.Message}", true);

            _status = OperationStatus.Failed;
            OnFailed();
        }
        finally
        {
            _isDone = true;
            _isValid = false;
            _state = OperationState.Finished;
            OnStateChanged(OperationState.Finished);

            if (_cancellationToken.IsCancellationRequested)
            {
                _status = OperationStatus.Canceled;
                OnCanceled();
            }
        }
    }

    private async Task<List<string>> FindUnusedGroups(CancellationToken cancellationToken)
    {
        var json = Resources.Load<TextAsset>("RemoteGroupIds");
        if (json == null)
        {
            Log("Json does not exist");
            return null;
        }

        var validAssetGroups = Helpers.ReadJson<List<string>>(json.text);

        Resources.UnloadAsset(json);

        if (validAssetGroups.IsNullOrEmpty())
        {
            Log("List of groups is empty");
            return null;
        }

        var remoteGroupsDirectory = GetRemoteAddressableAssetsLocation();
        if (!Directory.Exists(remoteGroupsDirectory))
        {
            Log($"{remoteGroupsDirectory} does not exist");
            return null;
        }

        var unknownSubdirectories = Directory.GetDirectories(remoteGroupsDirectory);
        if (unknownSubdirectories.IsNullOrEmpty())
        {
            Log("Cache is empty");
            return null;
        }

        var unusedGroups = new List<string>();

        Log("Search for unused assets...", true);
        foreach (var unknownSubdirectory in unknownSubdirectories)
        {
            if (cancellationToken.IsCancellationRequested) return unusedGroups;

            var subdirectories = Directory.GetDirectories(unknownSubdirectory);
            if (subdirectories.IsNullOrEmpty()) continue;

            foreach (var subdirectory in subdirectories)
            {
                var group = Path.GetFileNameWithoutExtension(subdirectory);
                if (validAssetGroups.Contains(group)) continue;
                var files = Directory.GetFiles(subdirectory);
                var isLock = false;
                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName == "__lock") isLock = true;
                }
                if (isLock) continue;
                unusedGroups.Add(subdirectories.Length > 1 ? subdirectory : unknownSubdirectory);
            }

            await Task.Yield();
        }

        if (unusedGroups.Count > 0)
        {
            Log($"Found {unusedGroups.Count} file(s)");
        }
        else
        {
            Log("No unused assets found");
        }

        return unusedGroups;
    }

    private string GetRemoteAddressableAssetsLocation()
    {
#if UNITY_ANDROID
        return Path.Combine(Application.persistentDataPath, "UnityCache/Shared");
#elif UNITY_IOS
        return Path.Combine(Application.persistentDataPath.Replace("Documents", "Library"), "UnityCache/Shared");
#else
        return string.Empty;
#endif
    }

    private async Task DeleteUnusedGroupsAsync(List<string> unusedGroups, CancellationToken cancellationToken)
    {
        if (unusedGroups.IsNullOrEmpty())
        {
            _percentComplete = 1.0F;
            OnRemovalAsset(_percentComplete);
            return;
        }

        Log("Deleting unused assets...");

        var total = unusedGroups.Count;
        var counter = 0;

        foreach (var unusedGroup in unusedGroups)
        {
            if (cancellationToken.IsCancellationRequested) return;

            Log($"Deleting \"{unusedGroup}\"");
            Directory.Delete(unusedGroup, true);

            counter++;
            _percentComplete = counter / (float)total;
            OnRemovalAsset(_percentComplete);

            await Task.Yield();
        }

        Log("All unused assets successfully deleted");
    }

    private void Log(string message, bool isError = false)
    {
        message = $"[AddressableAssetsCleaner]: {message}";

        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}

#if UNITY_EDITOR
public class AddressableAssetsHelperPreprocessing : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        SaveAddressableRemoteGroupIds();
    }

    private void SaveAddressableRemoteGroupIds()
    {
        var groupsLocalPath = AddressableAssetSettingsDefaultObject.Settings.RemoteCatalogBuildPath.GetValue(AddressableAssetSettingsDefaultObject.Settings);
        groupsLocalPath = groupsLocalPath.Remove(groupsLocalPath.LastIndexOf('/'));
        var groupsFullPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, groupsLocalPath);

        if (!Directory.Exists(groupsFullPath)) return;

        var subdirectories = Directory.GetDirectories(groupsFullPath);
        if (subdirectories.IsNullOrEmpty()) return;

        var groupIds = new List<string>();
        foreach (var file in Directory.GetFiles(subdirectories[^1]))
        {
            if (Path.GetExtension(file) != ".bundle") continue;
            var fileName = Path.GetFileNameWithoutExtension(file);
            var groupId = fileName[(fileName.LastIndexOf('_') + 1)..];
            groupIds.Add(groupId);
        }

        if (groupIds.Count == 0) return;
        var json = Helpers.CreateJson(groupIds);
        var jsonPath = Path.Combine(Application.dataPath, "Resources/RemoteGroupIds.json");
        File.WriteAllText(jsonPath, json);
        AssetDatabase.Refresh();
    }
}
#endif
