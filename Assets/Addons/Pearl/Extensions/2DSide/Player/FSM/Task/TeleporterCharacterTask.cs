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
    public class TeleporterCharacterTask : CharacterTask
    {
        [RequiredField]
        public BBParameter<Transform> destination = null;
        [RequiredField]
        public BBParameter<AxisX> facingDirection = null;

        protected override void Execute()
        {
            if (_currentCharacter != null && facingDirection != null && destination != null && destination.value != null)
            {
                _currentCharacter.transform.position = destination.value.position;
                _currentCharacter.ChangeFacingDirection(facingDirection.value);
            }
        }
    }
}
#endif