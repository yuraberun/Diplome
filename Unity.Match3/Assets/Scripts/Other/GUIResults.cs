using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GUIResults : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private TextMeshProUGUI _txt;
    [SerializeField] private CanvasGroup _canvasGroup;

    public static GUIResults Instance { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public async void ShowResultsAndRestartScene(string str, Color color, float delay)
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, 0.32f);
        _txt.text = str;
        _txt.color = color;

        await UniTask.Delay((int)(delay * 1000));
        await SceneLoader.LoadScene(SceneEnum.Main, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}