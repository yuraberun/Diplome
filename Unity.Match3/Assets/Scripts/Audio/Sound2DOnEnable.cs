using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class Sound2DOnEnable : MonoBehaviour
    {
        [SerializeField] private ActionType _audioType;

        public ActionType AudioType => _audioType;

        private void OnEnable()
        {
            Sounds.Play(_audioType);
        }
    }
}
