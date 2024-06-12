using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Extensions.Sensors;
using Unity.MLAgents.Integrations.Match3;
using Common;
using Sirenix.OdinInspector;
using Match3.App.Interfaces;
using Common.Interfaces;
using Match3.App;
using Match3.Core.Interfaces;
using Unity.VisualScripting;
using Match3.Core.Structs;
using Cysharp.Threading.Tasks;
using Common.GridTiles;
using Match3.App.Internal;
using System;
using System.Runtime.CompilerServices;
using Match3.Core;
using System.Linq;

public class Match3MLAgentsBoard : AbstractBoard
{
    [Title("Components")]
    [SerializeField] private AppContext _appContext;

    [Title("Settings")]
    [SerializeField] private int _rowsCount;
    [SerializeField] private int _columsCount;

    private BoardSize _boardSize;
    private UnityGameBoardRenderer _renderer;
    private UnityGame _unityGame;
    private Agent _agent;

    public static Match3MLAgentsBoard Instance;
    public static bool AI { get; private set; }

    private void Awake()
    {
        _boardSize = new BoardSize()
        {
            Rows = _rowsCount,
            Columns = _columsCount,
            NumCellTypes = 5,
            NumSpecialTypes = 0,
        };
    }

    public void Init()
    {
        Instance = this;
        _unityGame = _appContext.Resolve<UnityGame>();
        _renderer = _appContext.Resolve<IUnityGameBoardRenderer>() as UnityGameBoardRenderer;
        _agent = GetComponent<Agent>();
        FindObjectOfType<GUIHUD>().BtnActivateAI.SubscribeToClick(() =>
        {
            AI = !AI;

            if (AI)
            {
                _agent.RequestDecision();
            }
        });

        MovesCountController.OnLose += OnLose;
        LevelGoalController.OnWin += OnWin;

        if (AI && MovesCountController.CanMove && LevelGoalController.CanMove)
        {
            _agent.RequestDecision();
        }
    }

    private void OnDestroy()
    {
        MovesCountController.OnLose -= OnLose;
        LevelGoalController.OnWin -= OnWin;
        Instance = null;
    }

    public override int GetCellType(int row, int col)
    {
        if (_renderer != null)
        {
            var item = _renderer.GameBoardSlots[row, col];
            return item.ItemId;
        }

        return -1;
    }

    public override BoardSize GetMaxBoardSize()
    {
        return _boardSize;
    }

    public override int GetSpecialType(int row, int col)
    {
        return 0;
    }

    public override bool IsMoveValid(Move m)
    {
        return SimpleIsMoveValid(m);
    }

    public override bool MakeMove(Move m)
    {
        int startX = m.Row;
        int startY = m.Column;
        var moveEnd = m.OtherCell();
        int endX = moveEnd.Row;
        int endY = moveEnd.Column;

        //var a = _unityGame.GameBoard.GetGridSlots();
        //var temp = a[startX, startY];
        //a[startX, startY] = a[endX, endY];
        //a[endX, endY] = temp;

        //bool hasAny = false;

        //if (_unityGame.IsSolvedAny(out SolvedData<IUnityGridSlot> data))
        //{
        //    foreach (var i in data.SolvedSequences)
        //    {
        //        foreach (var ii in i.SolvedGridSlots)
        //        {
        //            if (ii.ItemId == LevelGoalController.ItemID)
        //            {
        //                _agent.AddReward(5f);
        //                hasAny = true;
        //            }
        //        }
        //    }
        //}

        //if (!hasAny)
        //{
        //    _agent.AddReward(-1f);
        //}

        //a[endX, endY] = a[startX, startY];
        //a[startX, startY] = temp;
        Swap(new GridPosition(startX, startY), new GridPosition(endX, endY));
        return true;
    }

    private async void Swap(GridPosition from, GridPosition to)
    {
        await _unityGame.SwapItemsAsync_c(from, to);

        if (AI && MovesCountController.CanMove && LevelGoalController.CanMove)
        {
            _agent.RequestDecision();
        }
    }

    public void OnAnySolved(SolvedData<IUnityGridSlot> solvedData)
    {
        foreach (var sequence in solvedData.SolvedSequences)
        {
            foreach (var item in sequence.SolvedGridSlots)
            {
                if (item.ItemId == LevelGoalController.ItemID)
                {
                    _agent.AddReward(1f);
                }
            }
        }
    }

    private void OnLose()
    {
        _agent.EndEpisode();
    }

    private void OnWin()
    {
        _agent.EndEpisode();
    }
}