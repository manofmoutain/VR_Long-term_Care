using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Manager;

namespace System
{
    public class AudioManager : Monosingleton<AudioManager>
    {
        public AudioSourceClass[] BGMSetting;
        AudioPlayer BGM;
        /// <summary>
        /// 音樂音量校正值
        /// </summary>
        [Range(0, 1)] float BGMVolumeValue = 0.5f;
        public AudioSourceClass[] SFXSetting;
        AudioPlayer SFX;
        /// <summary>
        /// 音效音量校正值
        /// </summary>
        [Range(0, 1)] float SFXVolumeValue = 0.5f;
        protected override void Awake()
        {
            base.Awake();
            BGM = new AudioPlayer(gameObject,"BGMPlayer",BGMSetting,BGMVolumeValue);
            SFX = new AudioPlayer(gameObject,"SFXPlayer",SFXSetting,SFXVolumeValue);
        }
        private void Start()
        {
            //BGMSetting = null;
            //SFXSetting = null;
        }

        #region BGM
        public void BGMPlay(string name)
        {
            BGM.Play(name);
        }
        public void BGMStop(string name)
        {
            BGM.Stop(name);
        }
        public void BGMStop()
        {
            BGM.StopAll();
        }
        public void BGMPause(string name)
        {
            BGM.Pause(name);
        }
        public void BGMResetFixValue(float value)
        {
            BGM.ResetFixValue(value);
        }
        public bool BGMIsPlaying()
        {
            return BGM.IsPlaying();
        }

        public float BGMTime()
        {
            return BGM.AudioTime();
        }

        public AudioClip BGMClip()
        {
            return BGM.GetAudioClip();
        }
        #endregion

        #region SFX
        public void SFXPlay(string name)
        {
            SFX.Play(name);
        }
        public void SFXStop(string name)
        {
            SFX.Stop(name);
        }
        public void SFXStop()
        {
            SFX.StopAll();
        }
        public void SFXPause(string name)
        {
            SFX.Pause(name);
        }
        public void SFXResetFixValue(float value)
        {
            SFX.ResetFixValue(value);
        }
        public bool SFXIsPlaying()
        {
            return SFX.IsPlaying();
        }

        public float SFXTime()
        {
            return SFX.AudioTime();
        }
        #endregion

        public void SetMute(bool set)
        {
            BGM.Mute(set);
            SFX.Mute(set);
        }
        public AudioClip SFXClip()
        {
            return SFX.GetAudioClip();
        }
    }
}

