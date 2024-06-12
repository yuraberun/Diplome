using System;
using Match3.App;

namespace Common.Interfaces
{
    public interface IGameUiCanvas
    {
        event EventHandler StartGameClick;
        event EventHandler<int> StrategyChanged;

        void ShowMessage(string message);
        void RegisterAchievedGoal(LevelGoal<IUnityGridSlot> achievedGoal);
    }
}