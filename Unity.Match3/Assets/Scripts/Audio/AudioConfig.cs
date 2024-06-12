using System.Collections;
using System.Collections.Generic;
using Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Config/Audio/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        #region Instance
        private static AudioConfig _instance;

        public static AudioConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load("Configs/AudioConfig") as AudioConfig;
                }

                return _instance;
            }
        }
        #endregion

        [TabGroup("UI", TabLayouting = TabLayouting.Shrink, TextColor = "green"), ListDrawerSettings(ListElementLabelName = "Name"), SerializeField]
        private List<Item> _ui = new List<Item>();

        [TabGroup("Settings", TabLayouting = TabLayouting.Shrink, TextColor = "cyan")]
        public AudioMixerGroup groupMusic;
        [TabGroup("Settings")]
        public AudioMixerGroup groupSound;

        [TabGroup("Settings")]
        public float minMusicdB;
        [TabGroup("Settings")]
        public float maxMusicdB;
        [TabGroup("Settings")]
        public float minSouddB;
        [TabGroup("Settings")]
        public float maxSounddB;

        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Value")]
        public Dictionary<ActionType, Item> Dictionary = new Dictionary<ActionType, Item>();

        public void InitDictionary()
        {
            Dictionary = new Dictionary<ActionType, Item>();

            foreach (Item item in _ui)
            {
                Dictionary.Add(item.type, item);
            }
        }

        [System.Serializable]
        public class Item
        {
            public ActionType type;
            [SerializeReference] public IActionController controller;

            public string Name()
            {
                return type.ToString();
            }
        }
    }
}
