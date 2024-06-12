using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public static class AudioEffects
    {
        private static AudioMixerGroup _groupMusic => AudioConfig.Instance.groupMusic;

        public static void Init()
        {
        }

        #region Controller Events
        public static void OnSceneUnload()
        {
            StopMusicLowpass();
        }
        #endregion

        #region Lowpass
        private static Coroutine _coroutineLowpass;

        public static void SetMusicLowpass(int value, float time)
        {
            _coroutineLowpass = CoroutinesBehaviour.Start(MusicLowpass(value, time));
        }

        public static void SetMusicLowpass(int value)
        {
            _groupMusic.audioMixer.SetFloat("Lowpass_hz", value);
        }

        public static void StopMusicLowpass()
        {
            CoroutinesBehaviour.Stop(_coroutineLowpass);
        }

        private static IEnumerator MusicLowpass(int value, float time)
        {
            float elapsedTime = 0f;
            _groupMusic.audioMixer.GetFloat("Lowpass_hz", out float startValue);

            while (elapsedTime < time)
            {
                _groupMusic.audioMixer.SetFloat("Lowpass_hz", Mathf.Lerp(startValue, value, elapsedTime / time));

                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }

            _groupMusic.audioMixer.SetFloat("Lowpass_hz", value);

            _coroutineLowpass = null;
        }
        #endregion
    }
}
