using System;
using System.Linq;
using Common.Interfaces;
using Common.Models;
using Common.UiElements;
using Match3.App;
using Match3.App.Interfaces;
using UnityEngine;

namespace Common
{
    public class GameUiCanvas : MonoBehaviour, IGameUiCanvas
    {
        [SerializeField] private AppContext _appContext;

        public event EventHandler StartGameClick;
        public event EventHandler<int> StrategyChanged;

        private void Start()
        {
        }

        public void ShowMessage(string message)
        {
            Debug.Log(message);
        }

        public void RegisterAchievedGoal(LevelGoal<IUnityGridSlot> achievedGoal)
        {
            ShowMessage($"The goal {achievedGoal.GetType().Name} achieved.");
        }
    }
}