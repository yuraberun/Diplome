using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Audio
{
    public interface IActionController
    {
        public void Init(AudioAction action, AudioActionParams parameters)
        {

        }

        public async UniTask InitAsync(AudioAction action, AudioActionParams parameters)
        {
            await UniTask.CompletedTask;
        }

        public void Play(AudioAction action, AudioActionParams parameters)
        {

        }

        public void Stop(AudioAction action, AudioActionParams parameters)
        {

        }

        public void Pause(AudioAction action, AudioActionParams parameters)
        {

        }

        public void Unpause(AudioAction action, AudioActionParams parameters)
        {

        }

        public void Kill(AudioAction action, AudioActionParams parameters)
        {

        }

        public async UniTask LoadAssetsInPreloader()
        {
            await UniTask.CompletedTask;
        }
    }

}
