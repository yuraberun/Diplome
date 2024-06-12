using System.Collections.Generic;
using System.Threading;
using Common.Interfaces;
using Common.Models;
using Cysharp.Threading.Tasks;
using Match3.App;
using Match3.App.Interfaces;
using Match3.Core.Structs;
using UnityEngine;

namespace Common
{
    public class UnityGame : Match3Game<IUnityGridSlot>
    {
        private readonly IInputSystem _inputSystem;
        private readonly IUnityGameBoardRenderer _gameBoardRenderer;

        private bool _isDragMode;
        private GridPosition _slotDownPosition;

        public UnityGame(IInputSystem inputSystem, IUnityGameBoardRenderer gameBoardRenderer,
            GameConfig<IUnityGridSlot> config) : base(config)
        {
            _inputSystem = inputSystem;
            _gameBoardRenderer = gameBoardRenderer;
        }

        protected override void OnGameStarted()
        {
            _inputSystem.PointerDown += OnPointerDown;
            _inputSystem.PointerDrag += OnPointerDrag;
        }

        protected override void OnGameStopped()
        {
            _inputSystem.PointerDown -= OnPointerDown;
            _inputSystem.PointerDrag -= OnPointerDrag;
        }

        public IEnumerable<IUnityGridSlot> GetGridSlots()
        {
            for (var rowIndex = 0; rowIndex < GameBoard.RowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < GameBoard.ColumnCount; columnIndex++)
                {
                    yield return GameBoard[rowIndex, columnIndex];
                }
            }
        }

        private void OnPointerDown(object sender, PointerEventArgs pointer)
        {
            if (IsPointerOnBoard(pointer.WorldPosition, out _slotDownPosition) && IsMovableSlot(_slotDownPosition))
            {
                _isDragMode = true;
            }
        }

        private void OnPointerDrag(object sender, PointerEventArgs pointer)
        {
            if (_isDragMode == false && !MovesCountController.CanMove && !LevelGoalController.CanMove)
            {
                return;
            }

            if (IsPointerOnBoard(pointer.WorldPosition, out var slotPosition) == false ||
                IsMovableSlot(slotPosition) == false)
            {
                _isDragMode = false;
                return;
            }

            if (IsSameSlot(slotPosition) || IsDiagonalSlot(slotPosition))
            {
                return;
            }

            _isDragMode = false;
            SwapItemsAsync(_slotDownPosition, slotPosition).Forget();
        }

        protected override async UniTask SwapItemsAsync(IBoardFillStrategy<IUnityGridSlot> fillStrategy, GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            await SwapGameBoardItemsAsync(position1, position2, cancellationToken);

            if (IsSolved(position1, position2, out var solvedData))
            {
                LevelGoalController.OnMoveEnd(solvedData);
                Match3MLAgentsBoard.Instance?.OnAnySolved(solvedData);
                NotifySequencesSolved(solvedData);
                await ExecuteJobsAsync(fillStrategy.GetSolveJobs(GameBoard, solvedData), cancellationToken);

                while (IsSolvedAny(out var solvedDataAll))
                {
                    LevelGoalController.OnMoveEnd(solvedDataAll);
                    Match3MLAgentsBoard.Instance?.OnAnySolved(solvedDataAll);
                    NotifySequencesSolved(solvedData);
                    await ExecuteJobsAsync(fillStrategy.GetSolveJobs(GameBoard, solvedDataAll), cancellationToken);
                }

                if (!LevelGoalController.CheckWin())
                {
                    MovesCountController.OnTurnUse();
                }
            }
            else
            {
                await SwapGameBoardItemsAsync(position1, position2, cancellationToken);
            }
        }

        private bool IsPointerOnBoard(Vector3 pointerWorldPosition, out GridPosition slotDownPosition)
        {
            return _gameBoardRenderer.IsPointerOnBoard(pointerWorldPosition, out slotDownPosition);
        }

        private bool IsMovableSlot(GridPosition gridPosition)
        {
            return GameBoard[gridPosition].IsMovable;
        }

        private bool IsSameSlot(GridPosition slotPosition)
        {
            return _slotDownPosition.Equals(slotPosition);
        }

        private bool IsDiagonalSlot(GridPosition slotPosition)
        {
            var isSideSlot = slotPosition.Equals(_slotDownPosition + GridPosition.Up) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Down) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Left) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Right);

            return isSideSlot == false;
        }
    }
}