using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioActionParams
    {
        public ActionType type = ActionType.None;
        public AudioConfig.Item config;
        public AudioSource source = null;
        public bool loop = false;
        public bool disableTimer = false;
        public bool canReleaseSource = true;
        public Transform container = null;
        public Vector3 position = Vector3.zero;
    }
}