#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.Audio;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks
{
    [Category("Pearl")]
    public class AudioManagerTask : ActionTask
    {
        public enum SoundManagerTaskEnum { SetClip, ChangePitch, StopMusic, ChangeSpeed, ChangeVolume }


        public BBParameter<bool> isMusic;
        [Conditional("isMusic", 0)]
        public BBParameter<AudioSourceManager> audioSource;

        public BBParameter<SoundManagerTaskEnum> action;

        [Conditional("action", (int)SoundManagerTaskEnum.SetClip)]
        public BBParameter<ComplexAudioClip> audioClip;
        [Conditional("action", (int)SoundManagerTaskEnum.ChangePitch)]
        public BBParameter<float> newPitch;
        [Conditional("action", (int)SoundManagerTaskEnum.ChangeSpeed)]
        public BBParameter<float> newSpeed;
        [Conditional("action", (int)SoundManagerTaskEnum.ChangeVolume)]
        public BBParameter<float> newVolume;
        [Conditional("action", (int)SoundManagerTaskEnum.ChangeVolume)]
        public BBParameter<float> timeTranslation;

        protected override void OnExecute()
        {
            ComplexAudioClip complexClip = audioClip.value;

            if (isMusic.value)
            {
                if (action.value == SoundManagerTaskEnum.ChangePitch)
                {
                    AudioManager.ChangePitchMusic(newPitch.value);
                }
                else if (action.value == SoundManagerTaskEnum.SetClip)
                {
                    AudioManager.PlayMusic(complexClip);
                }
                else if (action.value == SoundManagerTaskEnum.ChangeSpeed)
                {
                    AudioManager.ChangeSpeedMusic(newSpeed.value);
                }
                else if (action.value == SoundManagerTaskEnum.ChangeVolume)
                {
                    AudioManager.ChangeVolumeMusic(newVolume.value, timeTranslation.value);
                }
                else
                {
                    AudioManager.StopMusic();
                }
            }
            else if (audioSource.value != null)
            {
                AudioSourceManager aux = audioSource.value;

                if (action.value == SoundManagerTaskEnum.ChangePitch)
                {
                    aux.SetPicth(newPitch.value);
                }
                else if (action.value == SoundManagerTaskEnum.SetClip)
                {
                    aux.SetClip(complexClip, true);
                }
                else if (action.value == SoundManagerTaskEnum.ChangeSpeed)
                {
                    aux.SetSpeed(newSpeed.value);
                }
                else if (action.value == SoundManagerTaskEnum.ChangeVolume)
                {
                    aux.SetVolume(newVolume.value, timeTranslation.value);
                }
                else
                {
                    aux.Stop();
                }
            }
            EndAction();
        }
    }
}

#endif
