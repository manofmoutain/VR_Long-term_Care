using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Audio
{
    public class AudioPlayMethod
    {
        /// <summary>
        /// 播放清單
        /// </summary>
        Dictionary<string, AudioSourceClass> audioList = new Dictionary<string, AudioSourceClass>();

        /// <summary>
        /// 播放中的清單
        /// </summary>
        [SerializeField] List<GameObject> playingList = new List<GameObject>();

        /// <summary>
        /// 音量校正值
        /// </summary>
        [Range(0, 1)] float fixValue = 0.5f;

        /// <summary>
        /// 播放池的父物件
        /// </summary>
        GameObject playOBJ;

        /// <summary>
        /// 播放物件池
        /// </summary>
        GameObject audioPool;

        /// <summary>
        /// 靜音
        /// </summary>
        bool isMute = false;

        /// <summary>
        /// 初始化播放清單
        /// </summary>
        /// <param name="thisOBJ"></param>
        /// <param name="type">播放器類型名稱</param>
        /// <param name="playList">播放清單</param>
        /// <param name="fixvalue">初始音量較正值</param>
        public AudioPlayMethod(GameObject thisOBJ, string type, AudioSourceClass[] playList, float fixvalue)
        {
            for (int i = 0; i < playList.Length; i++)
            {
                var play = playList[i];
                audioList.Add(play.AudioName, play);
            }
            fixValue = fixvalue;


            #region 產生物件池
            playOBJ = new GameObject();
            playOBJ.transform.SetParent(thisOBJ.transform, false);
            playOBJ.name = type;

            audioPool = new GameObject();
            audioPool.transform.SetParent(playOBJ.transform, false);
            audioPool.name = "Pool";
            #endregion
        }

        /// <summary>
        /// 重設音量
        /// </summary>
        /// <param name="value"></param>
        public void RestVolumeValue(float value)
        {
            for (int i = 0; i < playingList.Count; i++)
            {
                var playing = playingList[i];
                float volumeValue = playing.GetComponent<AudioSource>().volume;
                //重設音量
                playing.GetComponent<AudioSource>().volume = (volumeValue / fixValue) * value;
            }
            //將新的音量賦予音量校正值
            fixValue = value;
        }

        /// <summary>
        /// 設定靜音
        /// </summary>
        /// <param name="active"></param>
        public void SetMute(bool active)
        {
            isMute = active;
            for (int i = 0; i < playingList.Count; i++)
            {
                var playing = playingList[i];
                playing.GetComponent<AudioSource>().mute = isMute;
            }
        }

        /// <summary>
        /// 一般播放
        /// </summary>
        /// <param name="name"></param>
        public void Play(string name)
        {
            //如果音樂清單中有
            if (audioList.ContainsKey(name))
            {
                PlayNextAudio(name);
            }
            else
            {
                Debug.LogError("音樂清單中沒有音樂");
            }
        }
        /// <summary>
        /// 暫停播放
        /// </summary>
        /// <param name="name"></param>
        public void Pause(string name)
        {
            //如果音樂清單中有
            if (playingList.Count>0)
            {
                PauseAudio(name);                
            }
            else
            {
                Debug.LogError("音樂清單中沒有音樂");
            }
        }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        /// <returns></returns>
        public bool Playing()
        {
            int playingCount = playingList.Count;
            //取得物件
            GameObject obj;
            AudioSource audioSource;

            if (playingCount > 0)//如果有正在播放的Audio
            {
                obj = playingList[0];
                audioSource = obj.GetComponent<AudioSource>();
                if (audioSource.isPlaying)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                obj = Create();
                playingList.Add(obj);
                audioSource = playingList[0].GetComponent<AudioSource>();
                if (audioSource.isPlaying)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 回傳Audio的時間長度
        /// </summary>
        /// <returns></returns>
        public float AudioTime()
        {
            int playingCount = playingList.Count;
            //取得物件
            GameObject obj;
            AudioSource audioSource;
            if (playingCount > 0)//如果有正在播放的Audio
            {
                obj = playingList[0];
                audioSource = obj.GetComponent<AudioSource>();
                if (audioSource.clip!=null)
                {
                    return audioSource.clip.length;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                obj = Create();
                playingList.Add(obj);
                audioSource = playingList[0].GetComponent<AudioSource>();
                if (audioSource.clip != null)
                {
                    return audioSource.clip.length;
                }
                else
                {
                    return 0;
                }
            }

        }

        /// <summary>
        /// 獲得播放片段
        /// </summary>
        /// <returns></returns>
        public AudioClip GetAudioClip()
        {
            int playingCount = playingList.Count;
            //取得物件
            GameObject obj;
            AudioSource audioSource;
            if (playingCount > 0)//如果有正在播放的Audio
            {
                obj = playingList[0];
                audioSource = obj.GetComponent<AudioSource>();
                if (audioSource.clip != null)
                {
                    return audioSource.clip;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                obj = Create();
                playingList.Add(obj);
                audioSource = playingList[0].GetComponent<AudioSource>();
                if (audioSource.clip != null)
                {
                    return audioSource.clip;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 產生播放器
        /// </summary>
        /// <param name="name"></param>
        public void AddAudioPlayer(string name)
        {
            if (audioList.ContainsKey(name))
            {
                AudioPlayFunction(name);
            }
            else
            {
                Debug.LogError("音樂清單中找不到音樂");
            }
        }

        /// <summary>
        /// 停止所有播放中的Audio並回收
        /// </summary>
        /// 
        public void StopAllAudio()
        {
            for (int i = 0; i < playingList.Count; i++)
            {
                var stop = playingList[i];
                Recycle(stop);
            }
        }
        /// <summary>
        /// 停止特定的Audio並回收
        /// </summary>
        /// <param name="name"></param>
        /// 
        public void StopPointAudio(string name)
        {
            for (int i = 0; i < playingList.Count; i++)
            {
                var stop = playingList[i];
                if (stop.name == name)
                {
                    Recycle(stop);
                }
            }
        }

        /// <summary>
        /// 檢查物件池是否有可用物件，沒有就產生一個，並回收物件(減少創造物)
        /// </summary>
        /// <returns></returns>
        GameObject Create()
        {
            Transform _transform = audioPool.transform;
            GameObject oBJ;
            if (_transform.childCount > 0)
            {
                oBJ = _transform.GetChild(0).gameObject;
            }
            else
            {
                oBJ = new GameObject();
                oBJ.AddComponent<AudioSource>();
            }
            if (oBJ.GetComponent<AudioSource>() == null)
            {
                oBJ.AddComponent<AudioSource>();
            }
            oBJ.transform.SetParent(playOBJ.transform, false);
            return oBJ;

        }

        /// <summary>
        /// 回收物件
        /// </summary>
        /// <param name="obj"></param>
        void Recycle(GameObject obj)
        {
            playingList.Remove(obj);
            obj.transform.SetParent(audioPool.transform, false);
            obj.SetActive(false);
        }

        /// <summary>
        /// 播放功能
        /// </summary>
        /// <param name="name"></param>
        void AudioPlayFunction(string name)
        {
            //取得物件池
            GameObject obj = Create();
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            obj.name = name;

            bool reTrigger = Set(audioList[name], ref audioSource);
            obj.SetActive(true);
            audioSource.Play();

            playingList.Add(obj);
            float life = audioSource.clip.length;

            //判斷是否循環播放，或者重複播放
            if (!reTrigger)
            {
                if (!audioList[name].loop)
                {
                    Sequence _delayCallback;
                    _delayCallback = DOTween.Sequence();
                    _delayCallback.InsertCallback(life, delegate
                     {
                         Recycle(obj);
                     });
                }
            }
            else
            {
                Sequence _delayCallback;
                _delayCallback = DOTween.Sequence();
                _delayCallback.InsertCallback(life, delegate
                {
                    AudioPlayFunction(name);
                });
            }
        }

        

        /// <summary>
        /// 同一個播放器，暫停播放
        /// </summary>
        /// <param name="name"></param>
        void PauseAudio(string name)
        {
            int playingCount = playingList.Count;
            //取得物件
            GameObject obj;
            AudioSource audioSource;
            if (playingCount > 0)//如果有正在播放的Audio
            {
                obj = playingList[0];
                audioSource = obj.GetComponent<AudioSource>();
            }
            else
            {
                obj = Create();
                playingList.Add(obj);
                audioSource = playingList[0].GetComponent<AudioSource>();
            }
            //載入設定檔並播放
            bool reTrigger;
            reTrigger = Set(audioList[name], ref audioSource);

            audioSource.Pause();

            if (reTrigger)
            {
                float life = audioSource.clip.length;
                life -= audioSource.time;

                delayNextPlay = DOTween.Sequence();
                delayNextPlay.InsertCallback(life, delegate
                {
                    PauseAudio(name);
                });
            }
        }

        Sequence delayNextPlay;

        /// <summary>
        /// 同一個播放器，播放下一首
        /// </summary>
        /// <param name="name"></param>
        void PlayNextAudio(string name)
        {
            int playingCount = playingList.Count;
            //取得物件
            GameObject obj;
            AudioSource audioSource;


            if (playingCount > 0)//如果有正在播放的Audio
            {
                obj = playingList[0];
                audioSource = obj.GetComponent<AudioSource>();
            }
            else
            {
                obj = Create();
                playingList.Add(obj);
                audioSource = playingList[0].GetComponent<AudioSource>();
            }
            //移除播放到下一首的Delay
            if (delayNextPlay != null)
            {
                delayNextPlay.Kill();
            }

            if (obj.name != name)
            {
                obj.name = name;
            }

            //是否重製播放時間
            float time;
            if (!audioList[name].resetTime)
            {
                time = audioSource.time + 0.01f;
            }
            else
            {
                time = 0;
            }

            //載入設定檔並播放
            bool reTrigger;
            reTrigger = Set(audioList[name], ref audioSource);

            audioSource.time = time;
            audioSource.Play();
            obj.SetActive(true);

            if (reTrigger && audioList[name].loop)
            {
                float life = audioSource.clip.length;
                life -= audioSource.time;

                delayNextPlay = DOTween.Sequence();
                delayNextPlay.InsertCallback(life, delegate
                 {
                     Play(name);
                 });
            }
        }

        /// <summary>
        /// 回傳reTrigger
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        bool Set(AudioSourceClass setting, ref AudioSource player)
        {
            bool reTrigger;
            if (setting.audioClips.Length > 1 && setting.loop)
            {
                reTrigger = true;
            }
            else
            {
                reTrigger = false;
            }


            AudioClip audioClip = getClip(setting.audioClips);
            float volume = getVolume(setting.volume);
            float pitch = getPitch(setting.pitch);

            player.clip = audioClip;
            player.loop = setting.loop;
            player.volume = volume;
            player.pitch = pitch;
            if (setting.mixerGroup)
            {
                player.outputAudioMixerGroup = setting.mixerGroup;
            }
            else
            {
                player.outputAudioMixerGroup = null;
            }

            if (isMute)
            {
                player.mute = isMute;
            }

            return reTrigger;

        }




        /// <summary>
        /// 取得播放音源(AudioClip)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        AudioClip getClip(AudioClip[] data)
        {
            AudioClip audioClip;
            int range = data.Length;

            if (range == 1)//陣列範圍為0
            {
                audioClip = data[0];
            }
            else if (range > 1)
            {
                System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
                int number = random.Next(range);
                audioClip = data[number];
            }
            else
            {
                audioClip = null;
                Debug.LogError("Out of range");
            }

            return audioClip;
        }

        /// <summary>
        /// 取得音量
        /// </summary>
        /// <param name="vol"></param>
        /// <returns></returns>
        float getVolume(AudioSourceClass.Volume vol)
        {
            float volume;
            if (!vol.isRandom)
            {
                volume = vol.volume;
            }
            else
            {
                volume = Random.Range(vol.max, vol.min);
            }
            //音量校正
            volume *= fixValue;

            return volume;
        }

        /// <summary>
        /// 取得音調
        /// </summary>
        /// <param name="_pitch"></param>
        /// <returns></returns>
        float getPitch(AudioSourceClass.Pitch _pitch)
        {
            float pitch;
            if (!_pitch.isRandom)
            {
                pitch = _pitch.pitch;
            }
            else
            {
                pitch = Random.Range(_pitch.max, _pitch.min);
            }
            return pitch;

        }





    }
}

