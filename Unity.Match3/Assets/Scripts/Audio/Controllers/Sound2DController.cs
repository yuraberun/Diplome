using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Audio
{
    public class Sound2DController : IActionController
    {
        public PlayType _playType = PlayType.Default;
        [SerializeField]
        public List<Clip2D> _clips = new List<Clip2D>();

        public void Init(AudioAction action, AudioActionParams parameters)
        {
            if (HasClip(out Clip2D clip))
            {
                InitAudioSource(action, parameters, clip, clip.clipRef.GetAsset());
            }
        }

        public async UniTask InitAsync(AudioAction action, AudioActionParams parameters)
        {
            if (HasClip(out Clip2D clip))
            {
                InitAudioSource(action, parameters, clip, await clip.clipRef.GetAssetAsync());
            }
        }

        private void InitAudioSource(AudioAction action, AudioActionParams parameters, Clip2D clip, AudioClip audioClip)
        {
            if (audioClip != null)
            {
                AudioSource source = (parameters.source != null) ? (parameters.source) : (SourcesPool.Get());
                source.outputAudioMixerGroup = AudioConfig.Instance.groupSound;
                source.clip = audioClip;
                source.volume = clip.volume;
                source.enabled = true;
                source.loop = parameters.loop;
                source.name = parameters.type.ToString();
                source.spatialBlend = 0f;
                action.Sources.Add(source);
            }
        }

        private bool HasClip(out Clip2D clip)
        {
            clip = null;

            if (_playType == PlayType.Default && _clips.Count > 0 && _clips[0].clipRef != null)
            {
                clip = _clips[0];
            }
            else if (_playType == PlayType.Random && _clips.Count > 0)
            {
                clip = _clips[UnityEngine.Random.Range(0, _clips.Count)];
            }

            return clip != null && clip.clipRef != null && !string.IsNullOrEmpty(clip.clipRef.AssetGUID);
        }

        public void Play(AudioAction action, AudioActionParams parameters)
        {
            Stop(action, parameters);

            if (action.HasAnySource(out AudioSource source))
            {
                source.Play();

                if (!parameters.disableTimer)
                {
                    action.PlayCor = CoroutinesBehaviour.Start(Timer(source.clip.length, action.Kill));
                }
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

                if (parameters.canReleaseSource)
                {
                    action.Sources.Remove(source);
                    SourcesPool.Release(source);
                }
            }

            Sounds.ReleaseAction(action);
        }

        public async UniTask LoadAssetsInPreloader()
        {
            foreach (var clip in _clips)
            {
                await clip.clipRef.GetAssetAsync();
            }
        }

        public IEnumerator Timer(float time, Action callback)
        {
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            callback?.Invoke();
        }
    }
}
