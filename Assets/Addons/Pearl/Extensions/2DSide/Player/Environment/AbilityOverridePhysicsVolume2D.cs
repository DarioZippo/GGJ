#if PEARL_2DSIDE

using Pearl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    /*
    public class AbilityOverridePhysicsVolume2D<T>: MonoBehaviour where T : struct
    {
        [SerializeField]
        private T info = default;
        [SerializeField]
        private Type ability = null;

        public void Enter(GameObject obj)
        {
            ChangeOverride(obj, TriggerType.Enter);
        }

        public void Exit(GameObject obj)
        {
            ChangeOverride(obj, TriggerType.Exit);
        }
        private void ChangeOverride(GameObject obj, TriggerType triggerType)
        {
            if (obj == null)
            {
                return;
            }

            var manager = obj.GetComponent<CharacterManager>();
            if (manager)
            {
                var script = manager.GetAbility(ability);

                if (script is CharacterAbilityWithOverride<T> scriptWithOverride)
                {
                    if (triggerType == TriggerType.Enter)
                    {
                        scriptWithOverride.AddOverrideInfo(info);
                    }
                    else
                    {
                        scriptWithOverride.RipristnateOriginInfo();
                    }
                }
            }
        }
    }
    */
}

#endif
