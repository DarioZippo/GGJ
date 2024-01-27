#if INK

using Pearl.Ink;
using System;
using UnityEngine;
using UnityEngine.Video;

namespace Pearl
{
    #region Structs
    [Serializable]
    public struct VideoClipWithSubtitle
    {
        public VideoClip videoClip;
        public StoryIndex storyIndex;

        public VideoClipWithSubtitle(VideoClip clip, StoryIndex storyIndex)
        {
            this.videoClip = clip;
            this.storyIndex = storyIndex;
        }
    }

    [Serializable]
    public struct ComplexVideoClipWithSubtitle
    {
        public float speedVideo;
        public VideoClipWithSubtitle[] clips;

        public ComplexVideoClipWithSubtitle(float speed, params VideoClipWithSubtitle[] clips)
        {
            this.clips = clips;
            this.speedVideo = speed;
        }

        public ComplexVideoClipWithSubtitle(params VideoClipWithSubtitle[] clips)
        {
            this.clips = clips;
            this.speedVideo = 1;
        }
        #endregion
    }

    public class VideoManagerWithSubtitle : VideoManager
    {
        #region Inspector fields
        [Header("Subtitle")]

        [SerializeField]
        private bool isSubtitle = false;
        [SerializeField, ConditionalField("@isSubtitle")]
        private InteractiveTextManager textManager = default;
        [SerializeField, ConditionalField("@isSubtitle")]
        private StoryIndex _storyIndex = default;
        #endregion

        #region Private fields
        private SubtitleVideo _subtitleManager;
        private StoryIndex[] _indexs;
        #endregion

        #region Property
        public StoryIndex Index { set { _storyIndex = value; } }
        #endregion

        #region UnityCallbacks
        protected override void Start()
        {
            if (textManager)
            {
                textManager.SetText(string.Empty);
            }

            base.Start();
        }

        protected void Update()
        {
            CheckSubtitle();
        }
        #endregion

        #region Public Methods
        public void SetVideos(ComplexVideoClipWithSubtitle complexClips, bool play = false)
        {
            isSubtitle = true;

            var indexs = complexClips.clips.CreateArray((x) => x.storyIndex);
            var clips = complexClips.clips.CreateArray((x) => x.videoClip);

            SetVideos(clips, indexs, play);
        }

        public void SetVideo(VideoClipWithSubtitle clip, bool play = false)
        {
            SetVideo(clip.videoClip, clip.storyIndex, play);
        }

        public void SetVideo(VideoClip clip, StoryIndex storyIndex, bool play = false)
        {
            var clips = new VideoClip[] { clip };
            var indexs = new StoryIndex[] { storyIndex };

            SetVideos(clips, indexs, play);
        }

        public void SetVideos(VideoClip[] clips, StoryIndex[] indexs, bool play = false)
        {
            isSubtitle = true;
            this._indexs = indexs;

            base.SeVideos(clips, play);
        }

        public override void SetVideo(bool play = false)
        {
            if (isSubtitle && _indexs != null && _currentIndex < _indexs.Length)
            {
                _storyIndex = _indexs[_currentIndex];
                _subtitleManager = new(_storyIndex);
            }

            base.SetVideo(play);
        }
        #endregion

        #region Private Methods
        private void CheckSubtitle()
        {
            if (textManager != null && _subtitleManager != null && isSubtitle &&
                _subtitleManager.GetSubtitle((float)videoPlayer.time, out string text, out float duration))
            {
                textManager.WaitBeetweenText = duration;
                textManager.SetText(text);
            }
        }
        #endregion
    }
}

#endif
