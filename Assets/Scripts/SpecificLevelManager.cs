using Pearl;
using Pearl.Events;
using Pearl.Input;
using UnityEngine;

namespace Game
{
    public class SpecificLevelManager : LevelManager
    {
        public Transform[] boundaries;
        public GameObject gameOverPage;
        public VideoManager videoManager;

        public static bool GetIstance(out SpecificLevelManager result)
        {
            return Singleton<SpecificLevelManager>.GetIstance(out result);
        }

        public static SpecificLevelManager GetSpecificIstance()
        {
            Singleton<SpecificLevelManager>.GetIstance(out SpecificLevelManager result);
            return result;
        }


        protected override void PearlAwake()
        {
            base.PearlAwake();
        }

        protected override void PearlStart()
        {
            base.PearlStart();

            InputManager.SetSwitchMap("Gameplay", true);
        }

        private void Update()
        {
        }

        protected override void PauseInternal()
        {
        }

        protected override void UnpauseInternal()
        {
        }

        protected override void ResetGamePrivate()
        {
            gameOverPage.SetActive(false);
            videoManager.Stop();

            PearlInvoke.WaitForMethod(0.1f, () => { SceneSystemManager.RepeatScene(); });
        }

        protected override void GameOverPrivate()
        {
            gameOverPage.SetActive(true);
            videoManager.Play();
        }

        public void NewRoad()
        {
            Debug.Log("Triggered");
            PearlEventsManager.CallEvent("OnNewRoad", PearlEventType.Trigger);
        }

        public void ResetBoldi()
        {
            ResetGame();
        }
    }
}
