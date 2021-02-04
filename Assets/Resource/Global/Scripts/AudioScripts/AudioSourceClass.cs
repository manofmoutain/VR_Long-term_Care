using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    /// <summary>
    /// AudioSouce屬性
    /// </summary>
    [System.Serializable]    
    public class AudioSourceClass
    {
        public AudioSourceClass()
        {
            AudioName = "";
            audioClips = new AudioClip[1];
            loop = false;
            resetTime = true;
            volume = new Volume();
            pitch = new Pitch();
        }
        /// <summary>
        /// 音量控制屬性
        /// </summary>
        [System.Serializable]
        public class Volume
        {
            [Range(0,1)] public float volume = 1;
            public bool isRandom;            
            [Range(0, 1)] public float min = 1;
            [Range(0, 1)] public float max = 1;
            public Volume()
            {
                volume = 1;
                isRandom = false;
                max = 1;
                min = 1;
            }
        }

        /// <summary>
        /// 音調控制屬性
        /// </summary>
        [System.Serializable]
        public class Pitch
        {
            [Range(-3,3)] public float pitch = 1;
            public bool isRandom;
            [Range(-3, 1)] public float min = 1;
            [Range(1, 3)] public float max = 1;
            public Pitch()
            {
                pitch = 1;
                isRandom = false;
                min = 1;
                max = 1;
            }
        }

        public string AudioName;
        public AudioClip[] audioClips;        
        public bool loop;
        public bool resetTime;
        public int priority;
        public Volume volume;
        public Pitch pitch;
        public AudioMixerGroup mixerGroup;
    }
}

