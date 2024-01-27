#if MOREMOUNTAINS_CORGIENGINE

using Game.Character;
using MoreMountains.CorgiEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl;
using Pearl.NodeCanvas;
using UnityEngine;

namespace Game.Character
{
    [Category("Game/Character")]
    public abstract class CharacterTask : ActionTask
    {
        [RequiredField]
        public BBParameter<TypeControl> type = null;

        [Conditional("type", (int)TypeControl.Player)]
        public BBParameter<int> player = null;
        [Conditional("type", (int)TypeControl.AI)]
        public BBParameter<CharacterManager> character = null;

        protected CharacterManager _currentCharacter;

        protected override void OnExecute()
        {
            if (type != null && character != null && player != null && type.value == TypeControl.Player)
            {
                _currentCharacter = CharacterManager.GetPlayer(player.value);
            }
            else if (character != null)
            {
                _currentCharacter = character.value;
            }

            Execute();

            EndAction();
        }

        protected abstract void Execute();
    }
}
#endif
