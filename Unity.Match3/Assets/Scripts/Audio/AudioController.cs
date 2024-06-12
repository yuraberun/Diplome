using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public class AudioController : IInitializerItem
    {
        public static AudioController Instance { get; private set; } = new AudioController();

        public async UniTask Init()
        {
            Logger.Log("AudioController", "Init");
            AudioConfig.Instance.InitDictionary();
            SourcesPool.Init();
            Music.Init();
            Sounds.Init();
            AudioEffects.Init();

            ResetSoundsVolume();
            ResetMusicVolume();

            await UniTask.CompletedTask;
        }

        public void OnSceneUnload()
        {
            Music.OnSceneUnload();
            Sounds.OnSceneUnload();
            AudioEffects.OnSceneUnload();
        }


        public void PauseAll()
        {
            Music.PauseAll();
            Sounds.PauseAll();
        }

        public void UnpauseAll()
        {
            Music.UnpauseAll();
            Sounds.UnpauseAll();
        }

        public void ResetSoundsVolume()
        {
            float value = Mathf.Lerp(AudioConfig.Instance.minSouddB, AudioConfig.Instance.maxSounddB, 1f);
            value = (value == AudioConfig.Instance.minSouddB) ? (-80f) : (value);
            AudioConfig.Instance.groupSound.audioMixer.SetFloat("Sound", value);
        }

        public void ResetMusicVolume()
        {
            float value = Mathf.Lerp(AudioConfig.Instance.minMusicdB, AudioConfig.Instance.maxMusicdB, 1f);
            value = (value == AudioConfig.Instance.minMusicdB) ? (-80f) : (value);
            AudioConfig.Instance.groupMusic.audioMixer.SetFloat("Music", value);
        }
    }
}
