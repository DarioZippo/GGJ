using Pearl;
using Pearl.Input;
using UnityEngine;

namespace Game
{
    public class SpecificLevelManager : LevelManager
    {
        protected override void PearlAwake()
        {
            base.PearlAwake();
        }

        protected override void PearlStart()
        {
            base.PearlStart();

            InputManager.SetSwitchMap("Gameplay", true);

            Debug.Log("ciao");
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
