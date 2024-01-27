using Pearl;
using Pearl.Events;
using Pearl.Input;

namespace Game
{
    public class SpecificLevelManager : LevelManager
    {
        public float currentGas = 0;

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

            currentGas = 0;
        }

        protected override void PearlStart()
        {
            base.PearlStart();

            InputManager.SetSwitchMap("Gameplay", true);
        }

        private void Update()
        {
            PearlEventsManager.CallEvent("OnGas", currentGas);
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
    }
}
