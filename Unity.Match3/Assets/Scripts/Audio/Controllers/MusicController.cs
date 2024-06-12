using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Audio
{
    public class MusicController : IActionController
    {
        [SerializeField] private List<Item> _items = new List<Item>();

        public void Init(AudioAction action, AudioActionParams parameters)
        {
            Item clipItem = _items[Random.Range(0, _items.Count)];

            if (clipItem.clipRef == null)
            {
                Logger.LogError("clipItem.clipRef == null");
                return;
            }

            InitAudioSource(action, parameters, clipItem, clipItem.clipRef.GetAsset());
        }

        public async UniTask InitAsync(AudioAction action, AudioActionParams parameters)
        {
            Item clipItem = _items[Random.Range(0, _items.Count)];

            if (clipItem.clipRef == null)
            {
                Logger.LogError("clipItem.clipRef == null");
                return;
            }

            InitAudioSource(action, parameters, clipItem, await clipItem.clipRef.GetAssetAsync());
        }

        private void InitAudioSource(AudioAction action, AudioActionParams parameters, Item clipItem, AudioClip clip)
        {
            AudioSource source = parameters.source;
            source.outputAudioMixerGroup = AudioConfig.Instance.groupMusic;
            source.clip = clip;
            source.volume = clipItem.volume;
            source.enabled = true;
            source.loop = true;
            source.name = parameters.type.ToString();
            source.spatialBlend = 0f;
            action.Sources.Add(source);
        }

        public void Play(AudioAction action, AudioActionParams parameters)
        {
            Stop(action, parameters);

            if (action.HasAnySource(out AudioSource source))
            {
                source.Play();
            }
        }

        public void Stop(AudioAction action, AudioActionParams parameters)
        {
            if (action.HasAnySource(out AudioSource source))
            {
                source.Stop();
            }
        }

        public void Pause(AudioAction action, AudioActionParams parameters)
        {
            if (action.HasAnySource(out AudioSource source))
            {
                source.Pause();
            }
        }

        public void Unpause(AudioAction action, AudioActionParams parameters)
        {
            if (action.HasAnySource(out AudioSource source))
            {
                source.UnPause();
            }
        }

        public void Kill(AudioAction action, AudioActionParams parameters)
        {
            if (action.HasAnySource(out AudioSource source))
            {
                source.Stop();
            }
        }

        public async UniTask LoadAssetsInPreloader()
        {
            await UniTask.CompletedTask;
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
