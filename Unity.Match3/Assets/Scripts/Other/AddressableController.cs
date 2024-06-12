using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class AddressableController : MonoBehaviour
{
    private const int MEMORY_SIZE = 4096;

    private const string ASSETBUNDLE_DATA_BASE_NEW = "PLAYERASSETDATABASEINFONEW";
    private const string LOAD_ASSETBUNDLE = "LOADASSETBUNDLELISTNEW";
    private const string LOAD_ASSETBUNDLE_VIDEO_PREVIEW = "LOADASSETBUNDLELISTVIDEOPREVIEW";

    private static AddressableController s_instance;

    private readonly List<string> _nonAddressableScenes = new() {
        "GUI",
        "Splash",
        "Preloader",
        "Garage_New_2023",
#if UNITY_EDITOR
        "TestTrack",
#endif
    };

    private List<string> _localScenes = new() { };
    private List<string> _remoteContents = new() { };

    private List<string> _localChapters = new()
    {
        "CHAPTER0_NAME",
        "CHAPTER1_NAME",
    };

    private List<string> _localCars = new()
    {
        "CARNAME_00",
        "CARNAME_01",
        "CARNAME_02",
        "CARNAME_03",
        "CARNAME_04",
        "CARNAME_05",
        "CARNAME_06",
        "CARNAME_07",
        "CARNAME_08",
        "CARNAME_09",
        "CARNAME_10",
        "CARNAME_11"
    };

    private List<string> _tags = new();
    private CancellationTokenSource _cancelToken = new();
    private CancellationTokenSource _cancelHideDownload = new();
    private AsyncOperationHandle _downloadElement;
    private bool _musicGroupIsLoaded;

    public static AddressableController Instance
    {
        get
        {
            if (s_instance == null)
            {
                var go = new GameObject("AddressableController");
                s_instance = go.AddComponent<AddressableController>();
                DontDestroyOnLoad(go);
            }
            return s_instance;
        }
    }

    public bool IsMusicDownloaded
    {
        get { return _musicGroupIsLoaded; }
    }

    private void OnDestroy()
    {
        _cancelHideDownload.Cancel();
    }

    public void Initialize()
    {
        //var text = Resources.Load<TextAsset>("AddressableData");
        //string[] tags = JsonConvert.DeserializeObject<string[]>(text.text);
        //_tags.AddRange(tags);
        //StartCoroutine(CheckDownloadMusicGroup());
        //if (!PlayerPrefs.HasKey(KEY_CLEAR_ALL_CACHE))
        //{
        //    Log($"Clear assetbundle cache {Caching.ClearCache()}");
        //    PlayerPrefs.SetInt(KEY_CLEAR_ALL_CACHE, 1);
        //    if (PlayerPrefs.HasKey(ASSETBUNDLE_DATA_BASE_NEW))
        //    {
        //        PlayerPrefs.DeleteKey(ASSETBUNDLE_DATA_BASE_NEW);
        //    }
        //    if (PlayerPrefs.HasKey(LOAD_ASSETBUNDLE))
        //    {
        //        PlayerPrefs.DeleteKey(LOAD_ASSETBUNDLE);
        //    }
        //    if (PlayerPrefs.HasKey(LOAD_ASSETBUNDLE_VIDEO_PREVIEW))
        //    {
        //        PlayerPrefs.DeleteKey(LOAD_ASSETBUNDLE_VIDEO_PREVIEW);
        //    }
        //}
        //if (SystemInfo.systemMemorySize >= MEMORY_SIZE)
        //{
        //    DownloadAllContent();
        //}
    }

    /// <summary>
    /// Load object from Addressables asynchronous.
    /// </summary>
    /// <param name="key">object name.</param>
    /// <typeparam name="T">object type.</typeparam>
    /// <returns>handle of loaded object.</returns>
    public AsyncOperationHandle<T> LoadAssetAsync<T>(string key)
    {
        return Addressables.LoadAssetAsync<T>(key);
    }

    /// <summary>
    /// Load object form Addressables. For local or downloaded objects only!
    /// </summary>
    /// <param name="key">object name.</param>
    /// <typeparam name="T">object type.</typeparam>
    /// <returns>loaded object.</returns>
    public T LoadAsset<T>(string key)
    {
        return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
    }

    /// <summary>
    /// Load several objects from Addressables.
    /// </summary>
    /// <param name="keys">objects names.</param>
    /// <typeparam name="T"> objects type.</typeparam>
    /// <returns>handle of loaded objects.</returns>
    public AsyncOperationHandle<IList<T>> LoadAssetsAsync<T>(List<string> keys)
    {
        return Addressables.LoadAssetsAsync<T>(keys, _ => { }, Addressables.MergeMode.Union);
    }

    /// <summary>
    /// Load several objects from Addressables. For local or downloaded objects only.
    /// </summary>
    /// <param name="keys">objects names.</param>
    /// <typeparam name="T">objects type.</typeparam>
    /// <returns>loaded objects.</returns>
    public IList<T> LoadAssets<T>(List<string> keys)
    {
        return Addressables.LoadAssetsAsync<T>(keys, _ => { }, Addressables.MergeMode.Union).WaitForCompletion();
    }

    /// <summary>
    /// Load several objects from Addressables. For local or downloaded objects only.
    /// </summary>
    /// <param name="keys">objects names.</param>
    /// <param name="handle">handle of objects.</param>
    /// <typeparam name="T">objects type.</typeparam>
    /// <returns>loaded objects.</returns>
    public IList<T> LoadAssets<T>(List<string> keys, out AsyncOperationHandle<IList<T>> handle)
    {
        handle = Addressables.LoadAssetsAsync<T>(keys, _ => { }, Addressables.MergeMode.Union);
        return handle.WaitForCompletion();
    }

    /// <summary>
    /// Load and instantiate object asynchronous.
    /// </summary>
    /// <param name="key">object name.</param>
    /// <param name="parent">parent transform for instantiated object.</param>
    /// <param name="instantiateInWorldSpace">option to retain world space when instantiated with a parent.</param>
    /// <param name="trackHandle">if true, Addressables will track this request to allow it to be released via the result object.</param>
    /// <returns>handle of instantiated object.</returns>
    public AsyncOperationHandle<GameObject> InstantiateAsync(string key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true)
    {
        return Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace, trackHandle);
    }

    /// <summary>
    /// Load and instantiate object from Addressables. For local or downloaded objects only!
    /// </summary>
    /// <param name="key">object name.</param>
    /// <param name="parent">parent transform for instantiated object.</param>
    /// <param name="instantiateInWorldSpace">option to retain world space when instantiated with a parent.</param>
    /// <returns>instantiated object.</returns>
    public GameObject Instantiate(string key, Transform parent, bool instantiateInWorldSpace)
    {
        return Addressables.InstantiateAsync(key, parent, instantiateInWorldSpace).WaitForCompletion();
    }

    /// <summary>
    /// Load and instantiate object from Addressables. For local or downloaded objects only!
    /// </summary>
    /// <param name="key">object name.</param>
    /// <param name="handle">handle of object.</param>
    /// <param name="parent">parent transform for instantiated object.</param>
    /// <param name="instantiateInWorldSpace">option to retain world space when instantiated with a parent.</param>
    /// <returns>instantiated object.</returns>
    public GameObject Instantiate(string key, Transform parent, bool instantiateInWorldSpace, out AsyncOperationHandle<GameObject> handle)
    {
        handle = Addressables.LoadAssetAsync<GameObject>(key);
        return Instantiate(handle.WaitForCompletion(), parent, instantiateInWorldSpace);
    }

    /// <summary>
    /// Load scene asynchronous.
    /// </summary>
    /// <param name="sceneName">scene name.</param>
    /// <returns>handle of loaded scene.</returns>
    public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single, bool activateOnLoad = true)
    {
        var operation = Addressables.LoadSceneAsync(sceneName, loadMode, activateOnLoad);

        return operation;
    }

    /// <summary>
    /// Load scene.
    /// </summary>
    /// <param name="sceneName">scene name.</param>
    /// <returns>scene instance.</returns>
    public SceneInstance LoadScene(string sceneName)
    {
        return Addressables.LoadSceneAsync(sceneName).WaitForCompletion();
    }

    public void Release<T>(AsyncOperationHandle<T> handle)
    {
        if (!handle.IsValid()) return;
        Addressables.Release(handle);
    }

    public void Release<T>(T obj)
    {
        Addressables.Release(obj);
    }

    public bool IsSceneInAddressables(string scene)
    {
        return _nonAddressableScenes.FindIndex(w => string.Equals(w, scene, StringComparison.Ordinal)) == -1;
    }

    public bool IsSceneRemote(string scene)
    {
        return _localScenes.FindIndex(w => string.Equals(w, scene, StringComparison.Ordinal)) == -1;
    }

    public bool IsChapterRemote(string pattern)
    {
        return _localChapters.FindIndex(w => string.Equals(w, pattern, StringComparison.Ordinal)) == -1;
    }
    public bool IsCarRemote(string pattern)
    {
        return _localCars.FindIndex(w => string.Equals(w, pattern, StringComparison.Ordinal)) == -1;
    }

    public async UniTask<bool> IsContentDownloaded(string key)
    {
        var handle = Addressables.GetDownloadSizeAsync(key);
        await handle.Task;
        return handle.Result == 0L;
    }

    public AsyncOperationHandle<long> CheckContentDownloaded(string objectName)
    {
        var size = Addressables.GetDownloadSizeAsync(objectName);
        return size;
    }

    public AsyncOperationHandle<long> CheckSizeDownloadAllContent()
    {
        var size = Addressables.GetDownloadSizeAsync(_tags);
        return size;
    }

    public AsyncOperationHandle<long> CheckContentDownloaded(List<string> objectsName)
    {
        var size = Addressables.GetDownloadSizeAsync(objectsName);
        return size;
    }

    public async void DownloadAddressablePackages(string key, Action onComplete, Action<string> onError, Action<float, float> onProgress)
    {
        try
        {
            _cancelToken = new CancellationTokenSource();
            _downloadElement = Addressables.DownloadDependenciesAsync(key);
            var downloadStatus = _downloadElement.GetDownloadStatus();
            while (!_cancelToken.IsCancellationRequested && !_downloadElement.IsDone)
            {
#if UNITY_EDITOR
                Debug.LogError($"{_downloadElement.GetDownloadStatus().DownloadedBytes}/{downloadStatus.TotalBytes}");
#endif
                onProgress?.Invoke(_downloadElement.GetDownloadStatus().DownloadedBytes, downloadStatus.TotalBytes);
                await UniTask.Yield();
            }
            if (_downloadElement.Status == AsyncOperationStatus.Succeeded)
            {
#if UNITY_EDITOR
                this.Log($"Download content complete.");
#endif
                onComplete?.Invoke();
                AddressableAssetsCleaner.StartCleanUp();
            }
            else if (!_cancelToken.IsCancellationRequested)
            {
#if UNITY_EDITOR
                this.Log($"Download content not succeeded, status {_downloadElement.Status}.");
#endif
                onError?.Invoke(key);
            }
        }
        catch (Exception ex)
        {
            this.Log(ex.Message);
            onError?.Invoke(key);
        }
    }

    public async void DownloadAddressablePackages(List<string> keys, Action onComplete, Action<List<string>> onError, Action<float, float> onProgress)
    {
        try
        {
            _cancelToken = new CancellationTokenSource();
            _downloadElement = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
            var downloadStatus = _downloadElement.GetDownloadStatus();
            while (!_cancelToken.IsCancellationRequested && !_downloadElement.IsDone)
            {
#if UNITY_EDITOR
                this.Log($"{_downloadElement.GetDownloadStatus().DownloadedBytes}/{downloadStatus.TotalBytes}");
#endif
                onProgress?.Invoke(_downloadElement.GetDownloadStatus().DownloadedBytes, downloadStatus.TotalBytes);
                await UniTask.Yield();
            }
            if (_downloadElement.Status == AsyncOperationStatus.Succeeded)
            {
#if UNITY_EDITOR
                this.Log($"Download content complete.");
#endif
                onComplete?.Invoke();
                AddressableAssetsCleaner.StartCleanUp();
            }
            else if (!_cancelToken.IsCancellationRequested)
            {
#if UNITY_EDITOR
                this.Log($"Download content not succeeded, status {_downloadElement.Status}.");
#endif
                onError?.Invoke(keys);
            }
        }
        catch (Exception ex)
        {
            this.Log(ex.Message);
            onError?.Invoke(keys);
        }
    }

    public async void DownloadAllAddressablePackage(Action onComplete, Action onError, Action<float, float> onProgress)
    {
        try
        {
            _cancelToken = new CancellationTokenSource();
            _downloadElement = Addressables.DownloadDependenciesAsync(_tags, Addressables.MergeMode.Union);
            var downloadStatus = _downloadElement.GetDownloadStatus();
            while (!_cancelToken.IsCancellationRequested && !_downloadElement.IsDone)
            {
#if UNITY_EDITOR
                Debug.LogError($"{_downloadElement.GetDownloadStatus().DownloadedBytes}/{downloadStatus.TotalBytes}");
#endif
                onProgress?.Invoke(_downloadElement.GetDownloadStatus().DownloadedBytes, downloadStatus.TotalBytes);
                await UniTask.Yield();
            }
            if (_downloadElement.Status == AsyncOperationStatus.Succeeded)
            {
#if UNITY_EDITOR
                this.Log($"Download content complete.");
#endif
                onComplete?.Invoke();
                AddressableAssetsCleaner.StartCleanUp();
            }
            else if (!_cancelToken.IsCancellationRequested)
            {
#if UNITY_EDITOR
                this.Log($"Download content not succeeded, status {_downloadElement.Status}.");
#endif
                onError?.Invoke();
            }
        }
        catch (Exception ex)
        {
            this.Log(ex.Message);
            onError?.Invoke();
        }
    }

    public void CancelDownload()
    {
        _cancelToken.Cancel();
        CoroutinesBehaviour.Instance.StopCoroutine(_downloadElement);
    }

    public AsyncOperationHandle DownloadAddressableHandler(string packetName)
    {
        var download = Addressables.DownloadDependenciesAsync(packetName);
        return download;
    }

    private IEnumerator CheckDownloadMusicGroup()
    {
        bool isDownload = false;
        while (!isDownload)
        {
            var music = Addressables.GetDownloadSizeAsync("RaceMusic");
            yield return music;
            if (music.Status == AsyncOperationStatus.Succeeded && music.Result == 0L)
            {
                isDownload = true;
                _musicGroupIsLoaded = true;
            }
            else
            {
                yield return new WaitForSeconds(5.0F);
            }
        }
    }

    private async void DownloadAllContent()
    {
        bool isDownloaded = false;
        float currentTime;
        float waitTime = 10.0F;
        var needDownload = false;
        this.Log("Starting download...");
        do
        {
            try
            {
                var needDownloadSize = Addressables.GetDownloadSizeAsync(_tags);
                while (!_cancelHideDownload.IsCancellationRequested && !needDownloadSize.IsDone)
                {
                    await UniTask.Yield();
                }
                if (!_cancelHideDownload.IsCancellationRequested)
                {
                    if (needDownloadSize.Status == AsyncOperationStatus.Succeeded)
                    {
                        if (needDownloadSize.Result > 0L)
                        {
                            needDownload = true;
                            var downloadContent = Addressables.DownloadDependenciesAsync(_tags, Addressables.MergeMode.Union);
                            while (!_cancelHideDownload.IsCancellationRequested && !downloadContent.IsDone)
                            {
                                await UniTask.Yield();
                            }
                            if (!_cancelHideDownload.IsCancellationRequested && downloadContent.IsDone)
                            {
                                isDownloaded = true;
                            }
                        }
                        else
                        {
                            isDownloaded = true;
                        }
                    }
                    else
                    {
                        currentTime = 0.0F;
                        while (!_cancelHideDownload.IsCancellationRequested && currentTime < waitTime)
                        {
                            currentTime += Time.deltaTime;
                            await UniTask.Yield();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                currentTime = 0.0F;
                while (!_cancelHideDownload.IsCancellationRequested && currentTime < waitTime)
                {
                    currentTime += Time.deltaTime;
                    await UniTask.Yield();
                }
            }
        } while (!isDownloaded && !_cancelHideDownload.IsCancellationRequested);
        if (isDownloaded)
        {
            if (needDownload)
            {
                AddressableAssetsCleaner.StartCleanUp();
            }

            this.Log("Download complete.");
        }
        else
            this.Log("Download error.");
    }

    private T ReadJson<T>(string path) where T : new()
    {
        var json = Resources.Load<TextAsset>(path);
        if (json == null)
        {
            this.LogError($"File \"{path}\" not found!");
            return new T();
        }
        var deserializeSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Newtonsoft.Json.Formatting.Indented };
        return JsonConvert.DeserializeObject<T>(json.text, deserializeSettings);
    }
}
