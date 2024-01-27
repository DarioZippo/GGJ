using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pearl;
using Pearl.Events;
using Pearl.Input;

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
