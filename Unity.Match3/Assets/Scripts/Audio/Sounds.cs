using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public static class Sounds
    {
        private static List<AudioAction> _actions = new List<AudioAction>();

        public static void Init()
        {

        }

        public static void OnSceneUnload()
        {
            foreach (var action in new List<AudioAction>(_actions))
            {
                action?.Kill();
            }

            _actions.Clear();
        }

        public static void PauseAll()
        {
            foreach (var action in _actions)
            {
                action?.Pause();
            }
        }

        public static void UnpauseAll()
        {
            foreach (var action in _actions)
            {
                action?.Pause();
            }
        }

        public static AudioAction Create(AudioActionParams parameters)
        {
            if (parameters.type == ActionType.None)
            {
                return null;
            }

            AudioAction audioAction = new AudioAction();
            audioAction.Init(parameters);
            _actions.Add(audioAction);
            return audioAction;
        }

        public static async UniTask<AudioAction> CreateAsync(AudioActionParams parameters)
        {
            if (parameters.type == ActionType.None)
            {
                return null;
            }

            AudioAction audioAction = new AudioAction();
            await audioAction.InitAsync(parameters);
            _actions.Add(audioAction);
            return audioAction;
        }

        public static void Play(AudioActionParams parameters)
        {
            if (parameters.type == ActionType.None)
            {
                return;
            }

            var action = Create(parameters);
            action.Play();
        }

        public static void Play(ActionType type)
        {
            Play(new AudioActionParams() { type = type });
        }

        public static void Play(ActionType type, Transform container)
        {
            Play(new AudioActionParams() { type = type, container = container });
        }

        public static void Play(ActionType type, Vector3 position)
        {
            Play(new AudioActionParams() { type = type, position = position });
        }

        public static void ReleaseAction(AudioAction audioAction)
        {
            _actions.Remove(audioAction);
        }
    }
}
