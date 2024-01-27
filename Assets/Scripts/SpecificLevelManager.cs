using Pearl;
using Pearl.Input;

namespace Game
{
    public class SpecificLevelManager : LevelManager
    {
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
    }
}
