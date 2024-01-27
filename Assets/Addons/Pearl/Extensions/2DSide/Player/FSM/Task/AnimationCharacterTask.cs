#if MOREMOUNTAINS_CORGIENGINE

using Game.Character;
using MoreMountains.CorgiEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl;
using Pearl.NodeCanvas;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Character
{
    [Category("Game/Character")]
    public class AnimationCharacterTask : CharacterTask
    {
        [RequiredField]
        public BBParameter<Dictionary<string, bool>> animations = null;

        protected override void Execute()
        {
            if (_currentCharacter != null && _currentCharacter.Animator != null && animations != null)                 
            {
                foreach (var pair in animations.value)
                {
                    _currentCharacter.Animator.SetBool(pair.Key, pair.Value);
                }
            }
        }
    }
}
#endif