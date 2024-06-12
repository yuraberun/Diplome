using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class SourcesPool
    {
        private static List<AudioSource> _sources = new List<AudioSource>();
        private static Transform _container;

        public static void Init()
        {
            _container = new GameObject("_AudioSources").transform;
            MonoBehaviour.DontDestroyOnLoad(_container.gameObject);
        }

        public static AudioSource Get()
        {
            if (_sources.Count > 0)
            {
                AudioSource audioSource = _sources[0];
                _sources.RemoveAt(0);
                return audioSource;
            }
            else
            {
                GameObject obj = new GameObject("AudioSource");
                AudioSource audioSource = obj.AddComponent<AudioSource>();
                obj.transform.SetParent(_container);
                audioSource.playOnAwake = false;
                audioSource.pitch = 1f;
                return audioSource;
            }
        }

        public static void Release(AudioSource audioSource)
        {
            if (audioSource != null)
            {
                audioSource.clip = null;
                audioSource.enabled = false;
                audioSource.transform.SetParent(_container);
                audioSource.gameObject.name = "Empty";
                _sources.Add(audioSource);
            }
        }
    }
}
