#if PEARL_2DSIDE

using Pearl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    public interface IEffectInZone
    {
        public void OnZone(TriggerType type, GameObject zone);
    }
}

#endif
