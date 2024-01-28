using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Game
{
    public class VidPlayer : MonoBehaviour
    {
        [SerializeField]
        string videoFileName;

        public void VideoPlay()
        {
            
            if (TryGetComponent<VideoPlayer>(out var videoPlayer)) 
            {
                string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
                videoPlayer.url = videoPath;
                videoPlayer.Play();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}