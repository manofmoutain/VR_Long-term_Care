using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{    public class AudioPlayer
    {
        AudioPlayMethod player;
        /// <summary>
        /// 初始化播放清單
        /// </summary>
        /// <param name="thisOBJ"></param>
        /// <param name="typeName">播放器類型名稱</param>
        /// <param name="bgmList">播放清單</param>
        /// <param name="fixValue">初始音量校正值</param>
        public AudioPlayer(GameObject thisOBJ , string typeName , AudioSourceClass[] bgmList , float fixValue)
        {
            player = new AudioPlayMethod(thisOBJ,typeName,bgmList,fixValue);
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            player.Play(name);
        }

        public void Pause(string name)
        {
            player.Pause(name);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="name"></param>
        public void Stop(string name)
        {
            player.StopPointAudio(name);
        }

        /// <summary>
        /// 停止所有音源的播放
        /// </summary>
        public void StopAll()
        {
            player.StopAllAudio();
        }

        /// <summary>
        /// 重設音量校正值
        /// </summary>
        /// <param name="value"></param>
        public void ResetFixValue(float value)
        {
            player.RestVolumeValue(value);
        }

        /// <summary>
        /// 靜音設置
        /// </summary>
        /// <param name="mute"></param>
        public void Mute(bool mute)
        {
            player.SetMute(mute);
        }

        public bool IsPlaying()
        {
            return player.Playing();
        }

        public float AudioTime()
        {
            return player.AudioTime();
        }

        public AudioClip GetAudioClip()
        {
            return player.GetAudioClip();
        }





    }
}

