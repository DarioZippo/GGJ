using Pearl.Events;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;


namespace Pearl.ClockManager
{
    public class TimerComponent : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField]
        private bool isRandomic;
        [SerializeField, ConditionalField("!@isRandomic")]
        private float time = 3f;
        [SerializeField, ConditionalField("@isRandomic")]
        private Vector2 timeRandomic = Vector2.one * 3;
        [SerializeField]
        private TimeType type = TimeType.Scaled;
        [SerializeField]
        private bool onStart = true;
        [SerializeField]
        private FloatEvent OnCompleteTimer;
        #endregion

        #region Private Fields
        private Timer _timer;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        private void Start()
        {
            if (onStart)
            {
                StartTimer();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_timer.IsFinish(out float delay))
            {
                OnCompleteTimer?.Invoke(delay);
                _timer.ResetOff();
            }
        }
        #endregion

        #region Private Fields
        public void StartTimer()
        {
            timeRandomic = !isRandomic ? new Vector2(time, time) : timeRandomic;
            _timer = new Timer(timeRandomic, true, type);
        }
        #endregion
    }

}