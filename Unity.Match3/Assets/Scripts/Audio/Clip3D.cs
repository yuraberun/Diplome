using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Audio
{
    [System.Serializable, MultiLineProperty(2)]
    public class Clip3D
    {
        public AssetReferenceT<AudioClip> clipRef;
        [Range(0f, 1f)] public float volume = 1f;
        public float minDistance;
        public float maxDistance;
    }
}
