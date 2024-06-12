using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public static class Music
    {
        private static AudioSource _source;
        private static AudioAction _action;

        public static void Init()
        {
            _source = SourcesPool.Get();
        }

        #region Controller Events
        public static void OnSceneUnload()
        {
            Kill();
        }

        public static void PauseAll()
        {
            Pause();
        }

        public static void UnpauseAll()
        {
            Unpause();
        }
        #endregion

        public static async UniTask InitGarage()
        {
            await UniTask.CompletedTask;
            //_action = await CreateAudioAction(ActionType.None);
        }

        public static async UniTask InitLevel()
        {
            await UniTask.CompletedTask;
            //_action = await CreateAudioAction(ActionType.UI_gameMusic);
        }

        public static void Play()
        {
            _action.Play();
        }

        private static async UniTask<AudioAction> CreateAudioAction(ActionType type)
        {
            var action = new AudioAction();
            await action.InitAsync(new AudioActionParams()
            {
                type = type,
                source = _source,
            });

            return action;
        }

        public static void Pause()
        {
            _action?.Pause();
        }

        public static void Unpause()
        {
            _action?.Unpause();
        }

        public static void Kill()
        {
            _action?.Kill();
            _action = null;
        }
    }
}
