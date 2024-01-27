using Pearl.Events;
using Pearl.Debugging;
using Pearl.Tweens;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Pearl
{
    [Serializable]
    public struct ComplexAudioClip
    {
        public AudioClip[] clips;

        public ComplexAudioClip(params AudioClip[] clips)
        {
            this.clips = clips;
        }
    }

    [RequireComponent(typeof(AudioSource))]

    public class AudioSourceManager : MonoBehaviour, IReset, IPause
    {
        #region Insepctor
        [SerializeField]
        private AudioSource audioSource = null;
        [SerializeField]
        private bool stopSoundInPause = true;
        [SerializeField]
        private bool randomSequenceClip = false;

        public UnityEvent OnFinishClip = null;
        public UnityEvent OnFinishPieceOfClip = null;
        #endregion

        #region private fields
        private bool _isPause = false;
        private AudioClip[] _clips;
        private bool _sequences = false;
        private int _currentIndex = 0;
        private bool _sourceLoop = false;
        private bool _start;
        private TweenContainer _tweenVolume;
        private TweenContainer _tmpTween;
        private AudioSource _tmpSource;
        #endregion

        #region Properties
        public bool IsPause { get { return _isPause; } }

        public bool Loop { set { _sourceLoop = value; } }

        public bool IsPlaying
        {
            get
            {
                if (audioSource)
                {
                    return audioSource.isPlaying;
                }
                return false;
            }
        }


        public AudioMixerGroup OutputAudioMixerGroup
        {
            get
            {
                if (audioSource != null)
                {
                    return audioSource.outputAudioMixerGroup;
                }
                return null;
            }
            set
            {
                if (audioSource != null)
                {
                    audioSource.outputAudioMixerGroup = value;
                }
            }
        }

        public float Percent
        {
            get
            {
                if (audioSource != null && audioSource.clip != null)
                {
                    return audioSource.time / audioSource.clip.length;
                }
                return 0;
            }
        }

        public bool StopSoundInPause
        {
            get
            {
                return stopSoundInPause;
            }
            set
            {
                if (gameObject.activeSelf)
                {
                    if (!stopSoundInPause && value)
                    {
                        PearlEventsManager.AddAction<bool>(ConstantStrings.Pause, Pause);
                    }
                    else if (stopSoundInPause && !value)
                    {
                        PearlEventsManager.RemoveAction<bool>(ConstantStrings.Pause, Pause);
                    }
                }

                stopSoundInPause = value;
            }
        }
        #endregion

        #region UnityCallbacks
        protected void Awake()
        {
            if (audioSource)
            {
                _sourceLoop = audioSource.loop;
            }

            var sources = GetComponents<AudioSource>();
            audioSource = sources.IsAlmostSpecificCount() ? sources[0] : null;
            _tmpSource = sources.IsAlmostSpecificCount(1) ? sources[1] : null;
        }

        protected void OnDestroy()
        {
            _tweenVolume?.Kill();
            _tmpTween?.Kill();
        }

        protected void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        protected void Update()
        {
            if (_start && (!IsWorking() || IsFinish()))
            {
                FinishClipCallback();
            }
        }

        protected void OnEnable()
        {
            if (stopSoundInPause)
            {
                PearlEventsManager.AddAction<bool>(ConstantStrings.Pause, Pause);
            }
        }

        protected void OnDisable()
        {
            if (stopSoundInPause)
            {
                PearlEventsManager.RemoveAction<bool>(ConstantStrings.Pause, Pause);
            }
        }
        #endregion

        #region Public Methods
        public void DestroyAudioSource()
        {
            GameObjectExtend.DestroyExtend(gameObject);
        }

        public bool IsFinish()
        {
            return Percent >= 1;
        }

        public bool IsWorking()
        {
            return IsPlaying || IsPause;
        }

        public void SetVolume(float newVolume, float timeTranslation = 0, FunctionEnum function = FunctionEnum.Linear)
        {
            _tweenVolume?.ForceFinish();
            _tweenVolume = TweensExtend.VolumeTween(audioSource, timeTranslation, true, function, ChangeModes.Time, newVolume);
            _tweenVolume.Play();
        }

        public void Play(float timeCross = 0, FunctionEnum functionCross = FunctionEnum.Linear)
        {
            if (audioSource)
            {
                audioSource.Play();
                _start = true;

                if (timeCross > 0)
                {
                    var volume = audioSource.volume;
                    audioSource.volume = 0;
                    SetVolume(volume, timeCross, functionCross);
                }

            }
        }

        public void SetClip(bool play = false, float timeCross = 0, FunctionEnum functionCross = FunctionEnum.Linear)
        {
            if (timeCross > 0)
            {
                if (_tmpSource == null)
                {
                    _tmpSource = gameObject.AddAndCopyComponent<AudioSource>(audioSource);
                }

                _tmpSource.Stop();
                (_tmpSource, audioSource) = (audioSource, _tmpSource);

                _tmpTween?.ForceFinish();
                _tmpTween = TweensExtend.VolumeTween(_tmpSource, timeCross, true, functionCross, ChangeModes.Time, 0);
                _tmpTween.Play();
            }


            if (_clips != null && _currentIndex < _clips.Length && audioSource != null)
            {
                audioSource.clip = _clips[_currentIndex];
            }

            if (play)
            {
                Play(timeCross, functionCross);
            }
        }

        public void SetClip(in AudioClip clip, bool play = false, float timeCross = 0, FunctionEnum functionCross = FunctionEnum.Linear)
        {
            if (audioSource && clip != null)
            {
                _sequences = false;
                _clips = new AudioClip[] { clip };
                audioSource.loop = _sourceLoop;
                _currentIndex = 0;
                SetClip(play, timeCross, functionCross);
            }
        }

        public void SetClips(AudioClip[] clips, bool play = false, float timeCross = 0, FunctionEnum functionCross = FunctionEnum.Linear)
        {
            if (audioSource && clips.IsAlmostSpecificCount())
            {
                if (clips.Length == 1)
                {
                    SetClip(clips[0], play, timeCross, functionCross);
                }
                else
                {
                    _sequences = true;
                    _clips = clips;
                    _sourceLoop = audioSource.loop;
                    audioSource.loop = false;
                    _currentIndex = randomSequenceClip ? UnityEngine.Random.Range(0, _clips.Length) : 0;
                    SetClip(play, timeCross, functionCross);
                }
            }
        }

        public void SetClip(ComplexAudioClip complexClips, bool play = false, float timeCross = 0, FunctionEnum functionCross = FunctionEnum.Linear)
        {
            SetClips(complexClips.clips, play, timeCross, functionCross);
        }

        public void SetPicth(float newPitch)
        {
            if (audioSource)
            {
                audioSource.pitch = newPitch;
            }

            if (_tmpSource)
            {
                _tmpSource.pitch = newPitch;
            }
        }

        public void SetSpeed(float newSpeed)
        {
            SetPicth(newSpeed);
            var mixerGroup = audioSource.outputAudioMixerGroup;
            if (!mixerGroup.audioMixer.SetFloat("musicPicther", 1f / newSpeed))
            {
                LogManager.LogWarning("The parameter is not enable for speed");
            }
        }

        public void Stop()
        {
            if (audioSource)
            {
                _tweenVolume?.Kill();
                _tmpTween?.ForceFinish();
                audioSource.Stop();
            }

            if (_tmpSource)
            {
                _tmpSource.Stop();
            }

            ResetAudioSource();
        }

        public void Pause(bool isPause)
        {
            _isPause = isPause;

            if (audioSource && stopSoundInPause)
            {
                if (isPause)
                {
                    _tweenVolume?.Pause(true);
                    _tmpTween?.Pause(true);
                    audioSource.Pause();

                    if (_tmpSource)
                    {
                        _tmpSource.Pause();
                    }
                }
                else
                {
                    _tweenVolume?.Pause(false);
                    _tmpTween?.Pause(false);
                    audioSource.UnPause();

                    if (_tmpSource)
                    {
                        _tmpSource.UnPause();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private void ResetAudioSource()
        {
            _start = false;
            _isPause = false;
            _currentIndex = 0;

            if (audioSource)
            {
                audioSource.loop = _sourceLoop;
            }

            if (_tmpSource)
            {
                _tmpSource.loop = _sourceLoop;
            }
        }

        private void FinishClipCallback()
        {
            OnFinishPieceOfClip?.Invoke();

            if (_sequences)
            {
                if (randomSequenceClip)
                {
                    _currentIndex = UnityEngine.Random.Range(0, _clips.Length);
                }
                else
                {
                    _currentIndex = _sourceLoop ? MathfExtend.ChangeInCircle(_currentIndex, 1, _clips.Length) : ++_currentIndex;
                }

                if (_sourceLoop || _currentIndex < _clips.Length)
                {
                    SetClip(true);
                }
                else
                {
                    FinishClip();
                }
            }
            else
            {
                FinishClip();
            }
        }

        private void FinishClip()
        {
            ResetAudioSource();
            OnFinishClip?.Invoke();
        }

        public void ResetElement()
        {
        }

        public void DisableElement()
        {
        }
        #endregion
    }
}