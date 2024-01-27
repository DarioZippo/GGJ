using UnityEngine;
using Pearl;
using Pearl.Input;
using Pearl.Debugging;
using Pearl.Events;

namespace Game
{
    public class SpecificLevelManager : LevelManager
    {
        public Transform[] boundaries;
        /*int roadCounter = 0;

        void Awake(){
            PearlEventsManager.AddAction("OnRoadsUpdate", OnRoadsUpdate);
        }

        void OnDisable(){
            PearlEventsManager.RemoveAction("OnRoadsUpdate", OnRoadsUpdate);
        }
        */
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
        }

        protected override void GameOverPrivate()
        {
        }
        /*
        public void OnRoadsUpdate(){
            Debug.Log("Counter aggiornato: " + roadCounter);
            roadCounter++;
        }
        */
        public void NewRoad(){
            Debug.Log("Triggered");
            PearlEventsManager.CallEvent("OnNewRoad", PearlEventType.Trigger);
        }
        /*
        public int GetRoadsCounter(){
            return roadCounter;
        }
        */
    }
}
