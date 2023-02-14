using SnakeGame;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Deftouch
{
    public class LevelTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;

        private double countDownTime;
        private bool isStarted = false;
        private DateTime pauseDateTime;

        #region Unity Methods
        private void OnEnable()
        {
            GlobalEventHandler.AddListener(EventID.EVENT_COLLIDED_TO_OBSTACLE, Callback_On_Stop_Level_Timer_Requested);
        }
        private void OnDisable()
        {
            GlobalEventHandler.RemoveListener(EventID.EVENT_COLLIDED_TO_OBSTACLE, Callback_On_Stop_Level_Timer_Requested);
        }
        private void Start()
        {
            SetTimer(90);//90 Seconds.
            StartTimer();
        }
        private void OnApplicationPause(bool pause)
        {
            if (isStarted)
            {
                if (pause)
                {
                    pauseDateTime = DateTime.Now;
                }
                else
                {
                    countDownTime -= (DateTime.Now - pauseDateTime).TotalSeconds;
                }
            }
        }
        #endregion Unity Methods

        #region Public Methods
        private void SetTimer(double time)
        {
            countDownTime = time;
            timerText.text = secondsToTimeFormatter(countDownTime);
        }
        private void StartTimer()
        {
            if (isStarted)
            {
                Debug.LogError($" Timer is already running..");
                return;
            }
            isStarted = true;
            StartCoroutine(nameof(UpdateTime));
        }

        private void StopTimer()
        {
            isStarted = false;
            StopCoroutine(nameof(UpdateTime));
        }
        private bool IsRunning()
        {
            return isStarted;
        }
        private double GetRemainingTime()
        {
            return countDownTime;
        }
        #endregion Public Methods

        #region Private Methods
        private IEnumerator UpdateTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (countDownTime <= 0)
                {
                    timerText.text = secondsToTimeFormatter(0);
                    GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_LEVEL_TIMER_COMPLETE);
                    StopTimer();
                    break;
                }
                countDownTime--;
                timerText.text = secondsToTimeFormatter(countDownTime);
            }
        }

        private string secondsToTimeFormatter(double seconds)
        {
            // Show minutes and seconds format
            return string.Format("{0:0}:{1:00}", Mathf.Floor((float)seconds / 60), Mathf.RoundToInt((float)seconds % 60));

        }
        #endregion Private Methods

        #region Callbacks

        private void Callback_On_Stop_Level_Timer_Requested(object args)
        {
            StopTimer();
        }

        private void Callback_On_Start_Level_Timer_Requested(object args)
        {
            StartTimer();
        }

        private void Callback_On_Set_Level_Timer_Requested(object args)
        {
            SetTimer((double)args);
        }

        #endregion
    }
}
