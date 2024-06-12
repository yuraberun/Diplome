using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Audio
{
    public class AudioAction
    {
        private IActionController _controller;
        private AudioActionParams _parameters;

        public List<AudioSource> Sources { get; set; }
        public Coroutine PlayCor { get; set; }
        public Coroutine TimerCor { get; set; }

        public void Init(AudioActionParams parameters)
        {
            Sources = new List<AudioSource>();
            _parameters = parameters;
            _parameters.config = AudioConfig.Instance.Dictionary[parameters.type];
            _controller = _parameters.config.controller;
            _controller.Init(this, parameters);
        }

        public async UniTask InitAsync(AudioActionParams parameters)
        {
            Sources = new List<AudioSource>();
            _parameters = parameters;
            _parameters.config = AudioConfig.Instance.Dictionary[parameters.type];
            _controller = _parameters.config.controller;
            await _controller.InitAsync(this, parameters);
        }

        public void Play()
        {
            _controller.Play(this, _parameters);
        }

        public void Stop()
        {
            _controller.Stop(this, _parameters);
        }

        public void Pause()
        {
            _controller.Pause(this, _parameters);
        }

        public void Unpause()
        {
            _controller.Unpause(this, _parameters);
        }

        public void Kill()
        {
            CoroutinesBehaviour.Stop(PlayCor);
            CoroutinesBehaviour.Stop(TimerCor);
            _controller.Kill(this, _parameters);
        }

        public bool HasAnySource(out AudioSource source)
        {
            if (Sources.Count > 0)
            {
                source = Sources[0];
                return true;
            }

            source = null;
            return false;
        }
    }
}
