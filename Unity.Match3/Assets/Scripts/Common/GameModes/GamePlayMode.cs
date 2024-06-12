using System;
using Common.Interfaces;
using Cysharp.Threading.Tasks;
using Match3.App;
using Match3.App.Interfaces;
using Match3.Infrastructure.Interfaces;

namespace Common.GameModes
{
    public class GamePlayMode : IGameMode, IDeactivatable
    {
        private readonly UnityGame _unityGame;
        private readonly IGameUiCanvas _gameUiCanvas;
        private readonly IBoardFillStrategy<IUnityGridSlot>[] _boardFillStrategies;

        public GamePlayMode(IAppContext appContext)
        {
            _unityGame = appContext.Resolve<UnityGame>();
            _gameUiCanvas = appContext.Resolve<IGameUiCanvas>();
            _boardFillStrategies = appContext.Resolve<IBoardFillStrategy<IUnityGridSlot>[]>();
        }

        public event EventHandler Finished
        {
            add => _unityGame.Finished += value;
            remove => _unityGame.Finished -= value;
        }

        public async void Activate()
        {
            _unityGame.LevelGoalAchieved += OnLevelGoalAchieved;
            _gameUiCanvas.StrategyChanged += OnStrategyChanged;

            _unityGame.SetGameBoardFillStrategy(GetSelectedFillStrategy());
            _gameUiCanvas.ShowMessage("Game started.");
            await _unityGame.StartAsync();
            UnityEngine.MonoBehaviour.FindAnyObjectByType<Match3MLAgentsBoard>().Init();
        }

        public void Deactivate()
        {
            _unityGame.LevelGoalAchieved -= OnLevelGoalAchieved;
            _gameUiCanvas.StrategyChanged -= OnStrategyChanged;

            _unityGame.StopAsync().Forget();
            _gameUiCanvas.ShowMessage("Game finished.");
        }

        private void OnLevelGoalAchieved(object sender, LevelGoal<IUnityGridSlot> levelGoal)
        {
            _gameUiCanvas.RegisterAchievedGoal(levelGoal);
        }

        private void OnStrategyChanged(object sender, int index)
        {
            _unityGame.SetGameBoardFillStrategy(GetFillStrategy(index));
        }

        private IBoardFillStrategy<IUnityGridSlot> GetSelectedFillStrategy()
        {
            return GetFillStrategy(0);
        }

        private IBoardFillStrategy<IUnityGridSlot> GetFillStrategy(int index)
        {
            return _boardFillStrategies[index];
        }
    }
}