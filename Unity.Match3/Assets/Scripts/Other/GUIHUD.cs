using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Models;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIHUD : MonoBehaviour
{
    [Title("Assets")]
    [SerializeField] private IconsSetModel _icons;

    [Title("Components")]
    [SerializeField] private AppContext _appContext;
    [SerializeField] private Match3MLAgentsBoard _mlAgents;
    [SerializeField] private CustomButton _btnRestart;
    [SerializeField] private CustomButton _btnActivateAI;
    [SerializeField] private Image _imgIcon;
    [SerializeField] private TextMeshProUGUI _txtMoves;
    [SerializeField] private TextMeshProUGUI _txtGoals;

    public CustomButton BtnActivateAI => _btnActivateAI;

    public static GUIHUD Instance { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        var unityGame = _appContext.Resolve<UnityGame>();

        _btnRestart.Enabled = false;
        _btnActivateAI.Enabled = false;
        _btnRestart.SubscribeToClick(async () => await SceneLoader.LoadScene(SceneEnum.Main, UnityEngine.SceneManagement.LoadSceneMode.Single));
        _btnRestart.Enabled = true;
        _btnActivateAI.Enabled = true;
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void UpdateGoalsIcon(int ID)
    {
        _imgIcon.sprite = _icons.Sprites[ID];
    }

    public void UpdateMovesCount(int count)
    {
        _txtMoves.text = count.ToString();
    }

    public void UpdateGoalsCount(int count)
    {
        _txtGoals.text = count.ToString();
    }
}
