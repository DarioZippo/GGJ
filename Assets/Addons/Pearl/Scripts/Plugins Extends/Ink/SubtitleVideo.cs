#if INK
using System.Collections.Generic;

namespace Pearl.Ink
{
    public class SubtitleVideo
    {
        #region Struct
        public struct SubtitleElement
        {
            public string text;
            public float time;
            public float duration;

            public SubtitleElement(string text, float time, float duration)
            {
                this.text = text;
                this.time = time;
                this.duration = duration;
            }
        }
        #endregion

        #region Private fields
        private const float _durationDefault = 3f;

        private readonly Queue<SubtitleElement> _subtitles = new();
        private readonly StoryExtend _story = new();
        private SubtitleElement _currentElement;
        private bool _finish;
        #endregion

        #region Constructors
        public SubtitleVideo(StoryIndex index)
        {
            CreateSubititle(index);
        }
        #endregion

        #region Public Methods
        public bool GetSubtitle(float time, out string text, out float duration)
        {
            text = null;
            duration = 0f;

            if (_subtitles == null || _finish)
            {
                return false;
            }

            if (time >= _currentElement.time)
            {
                duration = _currentElement.duration;
                text = _currentElement.text;

                if (_subtitles.Count == 0)
                {
                    _finish = true;
                }
                else
                {
                    _currentElement = _subtitles.Dequeue();
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Private Methods
        private void CreateSubititle(StoryIndex index)
        {
            if (_story == null)
            {
                return;
            }

            _story.Reset(index);

            if (!_story.CanContinue())
            {
                return;
            }

            do
            {
                _story.GetPieceOfStory(out var _currentPiece);

                float duration = _durationDefault;
                float time = -1f;

                if (_currentPiece.tags.IsNotNullAndTryGetValue("time", out var timeText))
                {
                    time = ConvertExtend.FromStringToFloat(timeText);
                }

                if (time < 0)
                {
                    continue;
                }

                if (_currentPiece.tags.IsNotNullAndTryGetValue("time", out var durationText))
                {
                    duration = ConvertExtend.FromStringToFloat(durationText);
                }

                if (duration < 0)
                {
                    duration = _durationDefault;
                }


                _subtitles.Enqueue(new SubtitleElement(_currentPiece.text, time, duration));
            }
            while (_story.CanContinue());

            _currentElement = _subtitles.Dequeue();
        }
        #endregion
    }
}
#endif
