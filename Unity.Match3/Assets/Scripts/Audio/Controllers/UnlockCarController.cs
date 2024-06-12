using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Audio
{
    public class UnlockCarController : IActionController
    {
        [SerializeField]
        private Item _sound1;
        [SerializeField]
        private Item _sound2;
        [SerializeField]
        private Item _soundLopped;

        public async UniTask InitAsync(AudioAction action, AudioActionParams parameters)
        {
            await InitOne(_sound1);
            await InitOne(_sound2);
            await InitOne(_soundLopped, true);

            async UniTask InitOne(Item item, bool loop = false)
            {
                AudioSource source = SourcesPool.Get();
                source.outputAudioMixerGroup = AudioConfig.Instance.groupSound;
                source.clip = await item.clipRef.GetAssetAsync();
                source.volume = item.volume;
                source.enabled = true;
                source.loop = loop;
                source.name = parameters.type.ToString();
                source.spatialBlend = 0f;
                action.Sources.Add(source);
            }
        }

        public void Play(AudioAction action, AudioActionParams parameters)
        {
            Stop(action, parameters);

            action.PlayCor = CoroutinesBehaviour.Start(PlayAnim(action, parameters));
        }

        public void Stop(AudioAction action, AudioActionParams parameters)
        {
            foreach (var source in action.Sources)
            {
                source.Stop();
            }
        }

        public void Pause(AudioAction action, AudioActionParams parameters)
        {
            foreach (var source in action.Sources)
            {
                source.Pause();
            }
        }

        public void Unpause(AudioAction action, AudioActionParams parameters)
        {
            foreach (var source in action.Sources)
            {
                source.UnPause();
            }
        }

        public void Kill(AudioAction action, AudioActionParams parameters)
        {
            foreach (var source in action.Sources)
            {
                source.Stop();
                SourcesPool.Release(source);
            }

            Sounds.ReleaseAction(action);
        }

        public async UniTask LoadAssetsInPreloader()
        {
            await UniTask.CompletedTask;
        }

        public IEnumerator PlayAnim(AudioAction action, AudioActionParams parameters)
        {
            action.Sources[0].Play();

            yield return Timer(action.Sources[0].clip.length, null);

            action.Sources[1].Play();
            action.Sources[2].Play();
        }

        public IEnumerator Timer(float time, Action callback)
        {
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            callback?.Invoke();
        }

        [System.Serializable]
        public class Item
        {
            public AssetReferenceT<UnityEngine.AudioClip> clipRef;
            [Range(0f, 1f)]
            public float volume = 1f;
        }
    }
}
