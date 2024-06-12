using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public class Sound3DOnStart : MonoBehaviour
    {
        [SerializeField] private ActionType _type;
        [SerializeField] private AudioSource _audioSource;

        public AudioAction AudioAction { get; private set; }
        public AudioSource AudioSource { get; private set; }
        public float StartVolume { get; private set; }

        public async UniTask Init()
        {
            AudioAction = await Sounds.CreateAsync(new AudioActionParams()
            {
                type = _type,
                container = transform.parent,
                position = transform.localPosition,
                source = _audioSource,
                loop = (_audioSource != null) ? (_audioSource.loop) : (false),
                canReleaseSource = false,
            });

            if (AudioAction.HasAnySource(out AudioSource source))
            {
                StartVolume = source.volume;
                AudioSource = source;
            }
        }

        private void OnDestroy()
        {
            AudioAction?.Kill();
        }
    }
}
