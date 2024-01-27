#if NODECANVAS && INK

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.Video;

namespace Pearl
{
    [Category("Pearl")]
    public class VideoManagerTask : ActionTask<Transform>
    {
        public BBParameter<bool> play;
        public BBParameter<VideoClip> videoClip;
        public BBParameter<bool> isSubititle;
        [Conditional("isSubititle", 1)]
        public BBParameter<StoryIndex> storyIndex;

        public BBParameter<bool> waitFinishVideo;
        public BBParameter<bool> useAgent = true;
        [Conditional("useAgent", 0)]
        public BBParameter<Pearl.NodeCanvas.ComponentReference<VideoManager>> container;

        private VideoManager _videoManager;


        protected override void OnExecute()
        {
            if (!useAgent.value && container.value != null)
            {
                _videoManager = container.value.Component;
            }
            else
            {
                _videoManager = agent.GetComponent<VideoManager>();
            }


            if (_videoManager == null)
            {
                return;
            }

            if (isSubititle.value && _videoManager is VideoManagerWithSubtitle videoManagerWithSubtitle)
            {
                videoManagerWithSubtitle.SetVideo(videoClip.value, storyIndex.value, play.value);
            }
            else
            {
                _videoManager.SetVideo(videoClip.value, play.value);
            }

        }
    }
}

#endif
