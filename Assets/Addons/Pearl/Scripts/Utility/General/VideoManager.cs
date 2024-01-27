using Pearl.Events;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Pearl
{
    #region Struct

    [Serializable]
    public struct ComplexVideoClip
    {
        public float speedVideo;
        public VideoClip[] clips;

        public ComplexVideoClip(float speed, params VideoClip[] clips)
        {
            this.clips = clips;
            this.speedVideo = speed;
        }

        public ComplexVideoClip(params VideoClip[] clips)
        {
            this.clips = clips;
            this.speedVideo = 1;
        }

        #endregion
    }

    //Classe che gestisce i video
    public class VideoManager : MonoBehaviour
    {
        #region Inspector fields
        [SerializeField]
        protected VideoPlayer videoPlayer = null;
        #endregion

        #region UnityEvents
        public UnityEvent OnFinish;
        public BoolEvent OnPause;
        #endregion

        #region Private Fields
        protected VideoClip[] _clips;
        protected bool _sequences = false;
        protected int _currentIndex = 0;
        protected bool sourceLoop = false;
        #endregion

        #region Property
        public float Time
        {
            get
            {
                if (videoPlayer != null)
                {
                    return (float)videoPlayer.time;
                }

                return -1f;
            }
        }
        #endregion

        #region UnityCallback
        protected virtual void Start()
        {
            if (videoPlayer)
            {
                videoPlayer.loopPointReached += OnEndVideo;
            }
        }

        protected void OnDestroy()
        {
            if (videoPlayer)
            {
                videoPlayer.loopPointReached -= OnEndVideo;
            }
        }
        #endregion

        #region Public Methods
        public virtual void SetVideo(bool play = false)
        {
            if (_clips != null && _currentIndex < _clips.Length && videoPlayer != null)
            {
                videoPlayer.clip = _clips[_currentIndex];
            }

            if (play)
            {
                Play();
            }
        }

        public virtual void SetVideo(in VideoClip clip, bool play = false)
        {
            if (videoPlayer && clip != null)
            {
                _sequences = false;
                _clips = new VideoClip[] { clip };
                _currentIndex = 0;
                SetVideo(play);
            }
        }

        public virtual void SetVideo(ComplexVideoClip complexClips, bool play = false)
        {
            if (videoPlayer != null)
            {
                videoPlayer.playbackSpeed = complexClips.speedVideo;
            }
            SeVideos(complexClips.clips, play);
        }

        public virtual void SeVideos(VideoClip[] clips, bool play = false)
        {
            if (videoPlayer && clips.IsAlmostSpecificCount())
            {
                sourceLoop = videoPlayer.isLooping;

                if (clips.Length == 1)
                {
                    SetVideo(clips[0], play);
                }
                else
                {
                    _sequences = true;
                    _clips = clips;
                    _currentIndex = 0;
                    SetVideo(play);
                }
            }
        }

        public virtual void Play()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }

        public void Stop()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                videoPlayer.targetTexture.Release();
            }
        }

        public void Pause(bool pause)
        {
            if (videoPlayer != null)
            {
                if (pause)
                {
                    videoPlayer.Pause();
                }
                else
                {
                    Play();
                }
            }

            OnPause?.Invoke(pause);
        }

        public void Release()
        {
            if (videoPlayer != null && videoPlayer.targetTexture.IsNotNull(out var targetTexture))
            {
                targetTexture.Release();
            }
        }
        #endregion

        #region Private Methods
        private void OnEndVideo(VideoPlayer videoPlayer)
        {
            if (_sequences)
            {
                _currentIndex = sourceLoop ? MathfExtend.ChangeInCircle(_currentIndex, 1, _clips.Length) : ++_currentIndex;

                if (sourceLoop || _currentIndex < _clips.Length)
                {
                    SetVideo(true);
                }
                else
                {
                    OnFinish?.Invoke();
                }
            }
            else
            {
                OnFinish?.Invoke();
            }
        }
        #endregion
    }
}
